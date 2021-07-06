using CodeParadise.Money;

namespace PoEAA_DataMapper
{
    internal class Person : DomainObject
    {
        public string LastName { get; }
        public string FirstName { get; }
        public int NumberOfDependents { get; }
        public Person(int id, string lastName, string firstName, int numberOfDependents)
        {
            Id = id;
            LastName = lastName;
            FirstName = firstName;
            NumberOfDependents = numberOfDependents;
        }

        public Money GetExemption()
        {
            Money baseExemption = Money.Dollars(1500d);
            Money dependentExemption = Money.Dollars(750d);
            return baseExemption.Add(dependentExemption.Multiply((double)NumberOfDependents));
        }
    }
}