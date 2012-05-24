using System.Collections.Generic;
using System.ServiceModel;

namespace HotelAdmin.Service
{
    [ServiceContract]
    public interface IFactTypeService
    {
        [OperationContract]
        IEnumerable<FactTypeModel> ListFactTypes();

        [OperationContract]
        IEnumerable<FactTypeModel> FindFactTypes(string query);

        [OperationContract]
        FactTypeModel GetFactType(string code);

        [OperationContract]
        int AddFactType(FactTypeModel factType);

        [OperationContract]
        void UpdateFactType(FactTypeModel factType);

    }
}