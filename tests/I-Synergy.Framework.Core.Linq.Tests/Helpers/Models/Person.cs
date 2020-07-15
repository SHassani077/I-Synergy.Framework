﻿
namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models
{
    public class Person
    {
        public int Id { get; set; }
        public int? NullableId { get; set; }
        public string Name { get; set; }
    }

    public class PersonAge
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
