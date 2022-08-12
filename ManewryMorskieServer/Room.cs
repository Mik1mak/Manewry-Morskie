using Microsoft.AspNetCore.SignalR;

namespace ManewryMorskie.Server
{
    public class Room
    {
        public bool IsWaitingForPlayers => clients.Count < 2;

        private readonly List<Client> clients = new();
        public IReadOnlyList<Client> Clients => clients;

        public Room AddClient(Client client)
        {
            if (!IsWaitingForPlayers)
                throw new InvalidOperationException("Too many clients in Room!");

            clients.Add(client);
            return this;
        }

        public Task RunGame()
        {
            throw new NotImplementedException();
        }
    }

    public class Rooms
    {
        private Dictionary<string, Room> randomRooms = new();
        private Dictionary<string, Room> namedRooms = new();
        private readonly int roomsLimits;

        public int RoomsCount => randomRooms.Count + namedRooms.Count;

        public Rooms(IConfiguration config)
        {
            roomsLimits = int.Parse(config["Rooms:Limit"]);
        }

        public async Task CreateRandomRoom(IGroupManager groups, Client creator)
        {
            await CreateRoom(groups, creator, new Guid().ToString(), randomRooms);
        }

        public async Task CreateRoom(string name, IGroupManager groups, Client creator)
        {
            if(namedRooms.ContainsKey(name))
                await creator.Kick("Pokój o podanej nazwie już istnieje. Proszę podać inną nazwę.");
            else
                await CreateRoom(groups, creator, name, namedRooms);
        }

        private async Task CreateRoom(IGroupManager groups, Client creator, string name, IDictionary<string, Room> rooms)
        {
            if (RoomsCount < roomsLimits)
            {
                await groups.AddToGroupAsync(creator.Id, name);
                rooms.Add(name, new Room{}.AddClient(creator));
            }
            else
            {
                await creator.Kick("Osiągnięto maksymalną ilość pokoi. Proszę spróbować później.");
            }
        }

        public async Task JoinToRoom(string name, IGroupManager groups, Client newClient)
        {
            if(namedRooms.ContainsKey(name))
            {
                if (namedRooms[name].IsWaitingForPlayers)
                    await Join(namedRooms[name], newClient, groups, name);
                else
                    await newClient.Kick("Pokój jest zajęty.");
            }
            else
            {
                await newClient.Kick("Szukany pokój nie istnieje.");
            }
        }

        public async Task JoinToRandomRoom(IGroupManager groups, Client newClient)
        {
            var randomRoom = randomRooms.Where(x => x.Value.IsWaitingForPlayers).FirstOrDefault();

            if(randomRoom.Value == default)
                await newClient.Kick("Brak wolnego losowego pokoju.");
            else
                await Join(randomRoom.Value, newClient, groups, randomRoom.Key);
        }
        
        private async Task Join(Room room, Client newClient, IGroupManager groups, string groupName)
        {
            await groups.AddToGroupAsync(newClient.Id, groupName);
            room.AddClient(newClient);

            if(!room.IsWaitingForPlayers)
                await room.RunGame();
        }
    }
}
