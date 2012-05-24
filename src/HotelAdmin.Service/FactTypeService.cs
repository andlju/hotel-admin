using System;
using System.Collections.Generic;
using System.Linq;
using HotelAdmin.Domain;
using Petite;

namespace HotelAdmin.Service
{
    public class FactTypeService : IFactTypeService
    {
        private readonly IObjectContext _objectContext;
        private readonly IFactTypeRepository _factTypeRepository;

        public FactTypeService(IObjectContext objectContext, IFactTypeRepository factTypeRepository)
        {
            _objectContext = objectContext;
            _factTypeRepository = factTypeRepository;
        }

        public IEnumerable<FactTypeModel> ListFactTypes()
        {
            return _factTypeRepository.List().
                Select(ft => new FactTypeModel()
                                 {
                                     Id = ft.Id,
                                     Code = ft.Code,
                                     Name = ft.Name
                                 }).ToArray();
        }

        public FactTypeModel GetFactType(string code)
        {
            
            return _factTypeRepository.Find(ft => ft.Code == code).
                Select(ft => new FactTypeModel() { Id = ft.Id, Code = ft.Code, Name = ft.Name }).SingleOrDefault();
        }

        public IEnumerable<FactTypeModel> FindFactTypes(string query)
        {
            return _factTypeRepository.Find(ft => ft.Name.Contains(query)).
                Select(ft => new FactTypeModel()
                                 {
                                     Id = ft.Id,
                                     Code = ft.Code,
                                     Name = ft.Name
                                 }).ToArray();
        }

        public int AddFactType(FactTypeModel factType)
        {
            var ft = new FactType() {Code = factType.Code, Name = factType.Name};
            _factTypeRepository.Add(ft);
            _objectContext.SaveChanges();

            return ft.Id;
        }

        public void UpdateFactType(FactTypeModel factType)
        {
            var ft = _factTypeRepository.Get(f => f.Id == factType.Id);
            if (ft == null)
                throw new InvalidOperationException(string.Format("No FactType found with Id {0}", factType.Id));

            ft.Name = factType.Name;
            ft.Code = factType.Code;

            _objectContext.SaveChanges();
        }
    }
}