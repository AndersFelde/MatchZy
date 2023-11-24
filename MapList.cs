using System.Diagnostics;

namespace Get5
{
    public class MapList
    {
        public List<string> maps = new();
        public MapList(List<string>? maps = null)
        {
            this.maps = maps ?? new List<string>();
        }
        public void Debug()
        {
            ChatMessage.SendConsoleMessage(string.Join(", ", maps));
        }
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

        public void RemoveAt(int index)
        {
            maps.RemoveAt(index);
        }

        public void Append(MapList mapList)
        {
            maps.AddRange(mapList.maps);
        }

        public void Update(MapList mapList)
        {
            this.maps = mapList.maps;
        }

        public int Count()
        {
            return maps.Count;
        }

        public override string ToString()
        {
            return string.Join(", ", maps);
        }
    }
}