using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class NameList {
	
	private static string[] names = new string[]{
		"Ruby",
		"William",
		"Chloe",
		"Jack",
		"Mia",
		"Ethan",
		"Olivia",
		"Oliver",
		"Isabella",
		"Lucas",
		"Charlotte",
		"Noah",
		"Sophie",
		"Lachlan",
		"Sienna",
		"Cooper",
		"Ella",
		"Thomas",
		"Emily",
		"James",
		"Ava",
		"Joshua",
		"Amelia",
		"Liam",
		"Lily",
		"Jacob",
		"Grace",
		"Samuel",
		"Zoe",
		"Benjamin",
		"Matilda",
		"Riley",
		"Sophia",
		"Max",
		"Lucy",
		"Alexander",
		"Emma",
		"Charlie",
		"Madison",
		"Xavier",
		"Isla",
		"Tyler",
		"Georgia",
		"Mason",
		"Jessica",
		"Ryan",
		"Abigail",
		"Harrison",
		"Isabelle",
		"Levi",
		"Hannah",
		"Isaac",
		"Lilly",
		"Jake",
		"Scarlett",
		"Jayden",
		"Summer",
		"Harry",
		"Chelsea",
		"Oscar",
		"Eva",
		"Daniel",
		"Evie",
		"Henry",
		"Zara",
		"Hunter",
		"Jasmine",
		"Jackson",
		"Savannah",
		"Eli",
		"Sarah",
		"Hayden",
		"Holly",
		"Luke",
		"Maddison",
		"Nate",
		"Alexis",
		"Logan",
		"Layla",
		"Blake",
		"Maya",
		"Matthew",
		"Ivy",
		"Aiden",
		"Imogen",
		"Sebastian",
		"Stella",
		"Nicholas",
		"Alice",
		"Michael",
		"Sofia",
		"Dylan",
		"Hayley",
		"Connor",
		"Elizabeth",
		"Zachary",
		"Molly",
		"Flynn",
		"Addison",
		"Elijah",
		"Bella",
		"Patrick",
		"Ellie",
		"Archie",
		"Alyssa",
		"Jordan",
		"Harper",
		"Joseph",
		"Willow",
		"Nathan",
		"Annabelle",
		"Hamish",
		"Paige",
		"Mitchell",
		"Poppy",
		"Angus",
		"Lara",
		"Caleb",
		"Madeline",
		"Chase",
		"Mikayla",
		"Jaxon",
		"Audrey",
		"Adam",
		"Amber",
		"Finn",
		"Mackenzie",
		"Kai",
		"Claire",
		"Callum",
		"Indiana",
		"Edward",
		"Anna",
		"Leo",
		"Jade",
		"Ashton",
		"Milla",
		"Beau",
		"Eliza",
		"Marcus",
		"Violet",
		"Bailey",
		"Phoebe",
		"George",
		"Isabel",
		"Christian",
		"Piper",
		"Zac",
		"Lola",
		"Ryder",
		"Charli",
		"Owen",
		"Amelie",
		"Luca",
		"Evelyn",
		"Charles",
		"Rose",
		"Anthony",
		"Hanah",
		"Jasper",
		"Caitlin",
		"Austin",
		"Alana",
		"David",
		"Mila",
		"Alex",
		"Samantha",
		"Jett",
		"Alexandra",
		"Andrew",
		"Leah",
		"Cameron",
		"Victoria",
		"Aidan",
		"Kayla",
		"Hugo",
		"Eve",
		"Toby",
		"Tahlia",
		"Aaron",
		"Charlie",
		"Seth",
		"Angelina",
		"Jonathan",
		"Eden",
		"Gabriel",
		"Amy",
		"Jesse",
		"Aaliyah",
		"Lincoln",
		"Lillian",
		"Archer",
		"Natalie",
		"Ali",
		"Gabriella",
		"Hudson",
		"Tahila",
		"Christopher",
		"Erin",
		"Darcy",
	};
	
	public static List<string> GetRandomisedNameList() {
		List<string> retV = new List<string>(names);
		retV.Shuffle();
		return retV;
	}
}