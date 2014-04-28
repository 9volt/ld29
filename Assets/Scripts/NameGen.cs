using UnityEngine;
using System.Collections;

public class NameGen : MonoBehaviour {
	static string[] given = {"Yellow", "Cellar", "Bugs" , "Green" ,"Bouncy", "Hopping", "Shiny" , "Happy" , "Cold" , "Hot", "Fast", "Big" , "Small" , "Little" , "Tiny" , "Orange" , "Slow" , "Speedy", "Angry" , "Friendly" , "Pied", "Portly" , "Homely" , "Comely", "Ugly" , "Pretty " , "Crying" ,"Furious" , "Funny" , "Pious" , "Pippi" , "Jumpy" , "Jealous" , "Lucky" , "Large" , "Lonely" , "Excited" , "Running", "Swimming" , "Sassy", "Shy" , "Fertile", "Delicate", "Sneaky", "Shady", "Dancing" , "Bursting", "Ancient", "Exploding" , "Unique", "Pink" ,"Fluffy", "Fuzzy" , "Purple" , "Dusty" , "Young", "Old", "Odd" , "Average" , "Great" , "Golden" , "Silver", "Blue","Red", "Tangy", "Spunky", "Twisting" , "Spinning" , "Deep" , "Peter", "Singing" , "Writing" , "Walking" , "Sleeping" , "Joyful" , "Cheerful" ,"Melancholy" , "Hungry" , "Chasing", "Clean", "Clingy",  "Searching" , "Twilight", "Lost" , "Lovely", "Zippy" , "Broken", "Chipper" , "Chaste", "Wacky" , "Wiley" , "Sly" , "Silky" , "Icy", "Burnt" , "Blushing" , "Best", "King", "Queen" , "Princess", "Prince", "Magical" , "Ghostly"};
	static string[] sur = {"Bunny", "Door", "Moon", "Sun", "Cart", "Tunnel" ,"Carrot", "Cabbage", "Dust" , "Rabbit" , "Lapin", "Usagi" , "Mammal", "Monster", "Bear", "Lion", "Grass", "Snow", "River", "Cat", "Wheat", "Piper", "Longstocking", "Summer", "Fall", "Winter", "Spring" , "Lop", "Rock", "Stone" , "Pebble", "Sand" , "Husk" , "House", "Hut", "Wind", "Breeze", "Buffalo", "Bug" , "Bottle" , "Cloud", "Star", "Ground", "Prayer", "Mouse", "Corn" , "Leaf", "Tree", "Log", "Lake", "Ocean", "Mage" , "Snowflake", "Foot" , "Tail" , "Face" , "Logic" , "Lady" , "Man", "Path" , "Room" , "Heart" , "Lung" , "Love" , "Butt" , "Tambourine" , "Trumpet" , "Drum", "Song" , "Sea", "Flute" , "Shovel", "Farm" , "Cottontail",  "Rain", "Shower", "Son" , "Daughter" , "Bubblegum" , "Stump" , "Field" , "Fur" , "Beauty", "Belle" , "Box" , "Acorn" , "Pecan" , "Flower" , "Daisy" , "Dream" ,"Dove" , "Dance" , "Dawn" , "Dusk" , "Sparkle" , "Sprout" , "Potato" , "Penguin", "Shrub", "Onion" , "Raddish" , "Warren", "Home", "Eye" , "Drop", "Turnip" , "Plow" , "Ox" , "Cow" , "Sky" , "Storm" , "Fire" , "Reflection" , "Mirror", "Raindrop" , "Parsnip", "Lettuce", "Plant", "Vegetable", "Popcorn" , "Tomato", "Ruby", "Basil", "Square", "Rectangle", "Triangle", "Clock", "Season", "Form", "Oat" , "Stem" , "Branch" , "Bundle" , "Hope", "Health" , "Day" , "Night", "Peace" , "Hop" , "Terrain", "Dirt" , "Tornado", "Hurricane", "Flood", "Poem" , "Verse", "Dandelion", "Tulip", "Wish" };



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public string getName(){
		string name = "";
		name += given[Random.Range(0, given.Length)];
		name += " " + sur[Random.Range(0, sur.Length)];
		return name;
	}
}
