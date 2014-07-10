using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using System.Xml;

namespace ConstructGameMediaRecorder
{
    public class Location
    {
        public double AltitudeInMeters { get; set; }
        public double LatitudeInDegrees { get; set; }
        public double LongitudeInDegrees { get; set; }
        public double VerticalAccuracyInMeters { get; set; }
        public double HorizontalAccuracyInMeters { get; set; }
        public DateTime TimeStamp { get; set; }

        public bool IsEmptyLocation
        {
            get
            {
                return AltitudeInMeters == 0.0 &&
                       LatitudeInDegrees == 0.0 &&
                       LongitudeInDegrees == 0.0 &&
                       VerticalAccuracyInMeters == 0.0 &&
                       HorizontalAccuracyInMeters == 0.0;
            }
        }

        public static List<Location> ParseLocationsFromXml(string xml)
        {
            List<Location> locations = new List<Location>();

            XDocument doc = XDocument.Parse(xml);
            var root = doc.Element("Locations");
            foreach (XElement locationElement in root.Elements("Location"))
            {
                Location location = ParseLocationFromXml(locationElement);
                locations.Add(location);
            }
            return locations;
        }

        public static Location ParseLocationFromXml(string xml)
        {
            XDocument doc = XDocument.Parse(xml);
            XElement locationElement = doc.Element("Location");
            Location location = ParseLocationFromXml(locationElement);
            return location;
        }

        public static string GetXmlForLocations(List<Location> locations)
        {
            XElement root = new XElement("Locations");
            foreach (Location location in locations)
            {
                XElement locationElement = GetXElementForLocation(location);
                root.Add(locationElement);
            }
            XDocument doc = new XDocument(root);
            return doc.ToString();
        }

        // force the single measurement into a collection and return XML for that collection
        public static string GetXmlForLocations(Location location)
        {
            List<Location> locations = new List<Location>();
            locations.Add(location);
            return GetXmlForLocations(locations);
        }

        public static string GetXmlForLocation(Location location)
        {
            XElement locationElement = GetXElementForLocation(location);
            XDocument doc = new XDocument(locationElement);
            return doc.ToString();
        }

        public static Location ParseLocationFromXml(XElement locationElement)
        {
            Location location = new Location();
            location.AltitudeInMeters = double.Parse(locationElement.Element("AltitudeInMeters").Value);
            location.LatitudeInDegrees = double.Parse(locationElement.Element("LatitudeInDegrees").Value);
            location.LongitudeInDegrees = double.Parse(locationElement.Element("LongitudeInDegrees").Value);
            location.VerticalAccuracyInMeters = double.Parse(locationElement.Element("VerticalAccuracyInMeters").Value);
            location.HorizontalAccuracyInMeters = double.Parse(locationElement.Element("HorizontalAccuracyInMeters").Value);

            // time stamp has to have UTC specified explicitly
            DateTime dateTime = DateTime.Parse(locationElement.Element("TimeStampUniversal").Value);
            location.TimeStamp = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            return location;
        }

        public static XElement GetXElementForLocation(Location location)
        {
            XElement locationElement = new XElement("Location");
            locationElement.Add(new XElement("AltitudeInMeters", location.AltitudeInMeters.ToString()));
            locationElement.Add(new XElement("LatitudeInDegrees", location.LatitudeInDegrees.ToString()));
            locationElement.Add(new XElement("LongitudeInDegrees", location.LongitudeInDegrees.ToString()));
            locationElement.Add(new XElement("HorizontalAccuracyInMeters", location.HorizontalAccuracyInMeters.ToString()));
            locationElement.Add(new XElement("VerticalAccuracyInMeters", location.VerticalAccuracyInMeters.ToString()));
            locationElement.Add(new XElement("TimeStampUniversal", location.TimeStamp.ToString("MM/dd/yyyy HH:mm:ss.fff")));
            return locationElement;
        }

        public static List<Location> RemoveDuplicates(List<Location> locations)
        {
            List<Location> filteredLocations = new List<Location>();

            Dictionary<string, bool> table = new Dictionary<string, bool>();
            foreach (Location location in locations)
            {
                string key = GetXmlForLocation(location);
                if (table.ContainsKey(key) == false)
                {
                    filteredLocations.Add(location);
                    table.Add(key, true);
                }
            }
            return filteredLocations;
        }




    }
}

