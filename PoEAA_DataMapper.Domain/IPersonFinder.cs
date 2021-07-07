using System.Collections.Generic;

namespace PoEAA_DataMapper.Domain
{
    public interface IPersonFinder
    {
        IList<Person> FinAll();
        Person Find(int id);
        IList<Person> FindByLastName2(string pattern);
    }
}
