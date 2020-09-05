using Imgeneus.DatabaseBackgroundService.Handlers;
using System.Collections.Concurrent;
using System.Runtime.InteropServices.ComTypes;

namespace Imgeneus.World.Game.Player
{
    public partial class Character
    {
        /// <summary>
        /// Dictionary of friends.
        /// </summary>
        public readonly ConcurrentDictionary<int, Friend> Friends = new ConcurrentDictionary<int, Friend>();

        /// <summary>
        /// Character from whom friend request was sent last time.
        /// </summary>
        private Character LastFriendRequester;

        /// <summary>
        /// Sends friend request to player.
        /// </summary>
        /// <param name="requester"></param>
        public void RequestFriendship(Character requester)
        {
            LastFriendRequester = requester;
            SendFriendRequest(requester);
        }

        /// <summary>
        /// Clears friend requester. Saves changes to database if needed.
        /// </summary>
        /// <param name="accepted">friendship was accepted or not</param>
        public void ClearFriend(bool accepted)
        {
            if (accepted)
            {
                _taskQueue.Enqueue(ActionType.SAVE_FRIENDS, Id, LastFriendRequester.Id);

                var friend = new Friend(LastFriendRequester.Id, LastFriendRequester.Name, LastFriendRequester.Class, true);
                Friends.TryAdd(LastFriendRequester.Id, friend);
                SendFriendAdd(LastFriendRequester);

                friend = new Friend(Id, Name, Class, true);
                LastFriendRequester.Friends.TryAdd(Id, friend);
                LastFriendRequester.SendFriendAdd(this);
            }

            LastFriendRequester.SendFriendResponse(accepted);
            LastFriendRequester = null;
        }

        /// <summary>
        /// Deletes character from friends list.
        /// </summary>
        public void DeleteFriend(int characterId)
        {
            if (Friends.TryRemove(characterId, out var friend))
            {
                _taskQueue.Enqueue(ActionType.DELETE_FRIENDS, Id, friend.Id);

                _gameWorld.Players.TryGetValue(characterId, out var friendPlayer);
                if (friendPlayer != null)
                    friendPlayer.SendFriendOnline(Id, false);

                SendFriendDelete(friend.Id);
            }
        }

        /// <summary>
        /// Notifies, that friend is online.
        /// </summary>
        public void FriendOnline(Character player)
        {
            Friends.TryGetValue(player.Id, out var friend);
            if (friend != null)
            {
                friend.IsOnline = true;
                SendFriendOnline(friend.Id, friend.IsOnline);
            }
        }

        /// <summary>
        /// Notifies, that friend is offline.
        /// </summary>
        public void FriendOffline(Character player)
        {
            Friends.TryGetValue(player.Id, out var friend);
            if (friend != null)
            {
                friend.IsOnline = false;
                SendFriendOnline(friend.Id, friend.IsOnline);
            }
        }
    }
}
