using System;
using System.Linq;
using DynamicData;
using DynamicData.Binding;

namespace Test
{
    public class Program
    {
		public class Person
		{
			public Person(string name, byte age, string sex)
			{
				Name = name;
				Age = age;
				Sex = sex;
			}

			public int Id { get; set; }
			public string Name { get; set; }
			public byte Age { get; set; }
			public string Sex { get; set; }
		}

		public class Sex
		{
			public Sex(string description, int peopleCount)
			{
				Description = description;
				PeopleCount = peopleCount;
			}

			public string Description { get; set; }
			public int PeopleCount { get; set; }
		}

	    public static void Main(string[] args)
	    {
		    var FemaleSex = new Sex("Female", 0);
		    var MaleSex = new Sex("Male", 0);
		    
		    var myList = new SourceList<Person>();
			
			myList.Edit(people =>
			{
				people.Add(new Person("Alex", 32, "Male"));
				people.Add(new Person("Sveta", 24, "Female"));
				people.Add(new Person("Yura", 63, "Male"));
			});

			myList.Add(new Person("Luda", 56, "Female"));

			var people = myList
				.Connect()
//				.Filter(person => person.Age < 50)
				.Sort(SortExpressionComparer<Person>.Ascending(person => person.Age))
				//.GroupOn(person => person.Sex)
				.Subscribe(changeSet =>
				{
					foreach(var change in changeSet)
					{
						if (change.Type == ChangeType.Range)
						{
							foreach (var itemChange in change.Range)
							{
								if (change.Reason == ListChangeReason.AddRange)
								{
									if (itemChange.Sex == "Female")
										FemaleSex.PeopleCount++;
									else
										MaleSex.PeopleCount++;
								}
							}
						}
						else
						{
							if (change.Reason == ListChangeReason.Add)
							{
								if (change.Item.Current.Sex == "Female")
									FemaleSex.PeopleCount++;
								else
									MaleSex.PeopleCount++;
							}
							else if (change.Reason == ListChangeReason.Remove)
							{
								if (change.Item.Current.Sex == "Female")
									FemaleSex.PeopleCount--;
								else
									MaleSex.PeopleCount--;
							}
						}
					}
				});
			

			// Updates
			myList.Add(new Person("Valera", 35, "Male"));
			myList.Add(new Person("Snejka", 5, "Female"));
			myList.Add(new Person("Diana", 23, "Female"));
			myList.Add(new Person("Tanya", 21, "Female"));
			myList.Remove(myList.Items.Where(person => person.Name == "Snejka").FirstOrDefault());


			Console.WriteLine($"{FemaleSex.Description} = {FemaleSex.PeopleCount}");
			Console.WriteLine($"{MaleSex.Description} = {MaleSex.PeopleCount}");



//			foreach(var personGroup in people.Items)
//			{
//				Console.WriteLine($"{personGroup.GroupKey}:");
//				foreach(var person in personGroup.List.Items)
//					Console.WriteLine($"\t{person.Name} - {person.Age}");
//			}



	    }
    }
}
