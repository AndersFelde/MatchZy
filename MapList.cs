namespace Get5
{
    public class MapList
    {
        public List<string> maps = new();
        public bool HasMap(string mapName)
        {
            return maps.Contains(mapName);
        }
        public bool Remove(string mapName)
        {
            return maps.Remove(mapName);
        }
        public void Add(string mapName)
        {
            maps.Add(mapName);
        }

        public void Update(MapList mapList)
        {
            this.maps = mapList.maps;
        }

        public int Count()
        {
            return maps.Count;
        }
    }
}