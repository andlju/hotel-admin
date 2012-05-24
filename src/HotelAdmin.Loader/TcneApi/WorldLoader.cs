using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using HotelAdmin.Service;
using Newtonsoft.Json;

namespace HotelAdmin.Loader.TcneApi
{
    public class World : GeographicalItem
    {

    }

    public class Area : GeographicalItem
    {
        public Location Location;
    }

    public class Region : GeographicalItem
    {

    }

    public class Country : GeographicalItem
    {

    }

    public class Resort : GeographicalItem
    {

    }

    public class Hotel : GeographicalItem
    {
        public Images Images;
        public Location Location;
        public decimal Rating;
        public FactItem[] Facts;

        public string ResortName;
    }

    public class Images
    {
        public ImageInstance[][] Main;
        public ImageInstance[][] Slideshow;
    }

    public class ImageInstance
    {
        public int Width;
        public int Height;
        public string Href;
        public string Title;
    }

    public class GeographicalItem
    {
        public string Name;
        public List<DescriptionItem> Description;
        public List<Child> Children;
    }

    public class Child
    {
        public string Title;
        public string Href;
        public string Rel;
        public string Type;
    }

    public class DescriptionItem
    {
        public string Text;
    }

    public class FactItem
    {
        public string Title;
        public string Value;
        public string Type;
    }

    public class Location
    {
        public double Latitude;
        public double Longitude;
    }

    public class WorldLoader
    {
        
        static T Download<T>(string url)
        {
            var apiKey = ConfigurationManager.AppSettings["api-key"];

            Console.WriteLine("Downloading: {0}", url);
            if (url.Contains("?"))
            {
                url += "&";
            }
            else
            {
                url += "?";
            }
            url += "api-key=" + apiKey;
            var req = WebRequest.Create(url);
            using (var resp = req.GetResponse())
            using (var stream = resp.GetResponseStream())
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<T>(jsonReader);
            }
        }

        static GeographicalItem DownloadItem(Child child)
        {
            var type = child.Type.Substring(21);
            switch (type)
            {
                case "region":
                    return Download<Region>(child.Href);
                case "area":
                    return Download<Area>(child.Href);
                case "country":
                    return Download<Country>(child.Href);
                case "resort":
                    return Download<Resort>(child.Href);
                case "hotel":
                    return Download<Hotel>(child.Href);
            }
            return null;
        }

        public static void LoadWorld(HotelService hotelService, FactTypeService factTypeService, int maxHotels)
        {
            var world = Download<World>("http://api.thomascook.se/v-1/geography/world");

            List<Hotel> hotels = new List<Hotel>();

            FindHotels(hotels, world, maxHotels);

            Console.WriteLine("\n\nAdding FactTypes");
            Dictionary<string, FactTypeModel> factTypeDictionary = new Dictionary<string, FactTypeModel>();
            foreach (var fact in hotels.SelectMany(h => h.Facts))
            {
                FactTypeModel model;
                if (!factTypeDictionary.TryGetValue(fact.Type, out model))
                {
                    model = factTypeService.GetFactType(fact.Type);
                    if (model == null)
                    {
                        model = new FactTypeModel() { Code = fact.Type, Name = fact.Title };
                        var id = factTypeService.AddFactType(model);
                        model.Id = id;
                    }
                    factTypeDictionary[fact.Type] = model;
                }
            }
            Console.WriteLine("\n\nAdding hotels");
            foreach (var hotel in hotels)
            {
                Console.WriteLine("Adding hotel {0}", hotel.Name);
                var id = hotelService.AddHotel(hotel.Name, hotel.Description.Select(d => d.Text).FirstOrDefault(), hotel.ResortName,
                                               hotel.Images != null && hotel.Images.Main != null
                                                   ? hotel.Images.Main.Select(
                                                       i =>
                                                       i.OrderByDescending(ii => ii.Width).Select(ii => ii.Href).FirstOrDefault())
                                                         .FirstOrDefault()
                                                   : null, hotel.Location != null ? hotel.Location.Latitude : 0,
                                               hotel.Location != null ? hotel.Location.Longitude : 0);

                Console.WriteLine("Setting facts for hotel {0}", hotel.Name);
                hotelService.SetHotelFacts(id, from f in hotel.Facts
                                               select
                                                   new FactModel(factTypeDictionary[f.Type].Id, f.Type, f.Title,
                                                                 f.Value.Trim() != "-" && f.Value.Trim().ToLower() != "false",
                                                                 f.Value.ToLower() == "false" || f.Value.ToLower() == "true"
                                                                     ? string.Empty
                                                                     : f.Value));
            }
        }

        private static void FindHotels(List<Hotel> hotels, GeographicalItem parent, int maxHotels)
        {
            if (hotels.Count > maxHotels)
                return;

            if (parent.Children == null)
                return;

            foreach (var child in parent.Children)
            {
                var geoItem = DownloadItem(child);
                if (geoItem is Hotel)
                {
                    ((Hotel)geoItem).ResortName = parent.Name;
                    hotels.Add((Hotel)geoItem);
                    Console.WriteLine("Found Hotel: " + geoItem.Name);
                }
                else
                {
                    FindHotels(hotels, geoItem, maxHotels);
                }
            }
        }

    }
}