using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarOfLords.Core.Models;

namespace WarOfLords.Core
{
    public class BattleManager
    {
        public static ConcurrentDictionary<string, string> CountryFederationMapDic = new ConcurrentDictionary<string, string>();
        public static ConcurrentDictionary<string, List<BattleTeam>> CountryBattleTeamsMapDic = new ConcurrentDictionary<string, List<BattleTeam>>();
        public static ConcurrentDictionary<string, CountryMessageQueue> CountryMessageQueueMapDic = new ConcurrentDictionary<string, CountryMessageQueue>();
    }

    public class CountryMessageQueue
    {
        private string Country;
        private string Federation;
        private ConcurrentQueue<string> messageQueue;
        private ILogger Logger = LoggerFactory.Logger;

        public CountryMessageQueue(string country, string federation)
        {
            messageQueue = new ConcurrentQueue<string>();
            Country = country;
            Federation = federation;
        }

        private const string CountryMessageFormat = "[{0}],\tCountry,\t{1}@{2},\t{3}";
        public void EnqueueMessage(string format, params object[] args)
        {
            var now = DateTime.UtcNow;
            string messagePart = string.Format(format, args);
            string message = string.Format(BattleTeamMessageFormat, now.ToString("o"), Country, Federation, messagePart);
            this.messageQueue.Enqueue(message);
        }

        private const string BattleTeamMessageFormat = "[{0}],\tBattleTeam,\t{1}@{2},\t{3}~{4},\t{5}";
        public void EnqueueMessage(BattleTeam source, string format, params object[] args)
        {
            var now = DateTime.UtcNow;
            string messagePart = string.Format(format, args);
            string message = string.Format(BattleTeamMessageFormat, now.ToString("o"), Country, Federation, source.Name, source.Id, messagePart);
            this.messageQueue.Enqueue(message);
        }

        private const string BattleUnitMessageFormat = "[{0}],\tBattleUnit,\t{1}@{2},\t{3}~{4},\t{5}~{6},\t{7}";
        public void EnqueueMessage(BattleUnit source, string format, params object[] args)
        {
            var now = DateTime.UtcNow;
            string messagePart = string.Format(format, args);
            string message = string.Format(BattleUnitMessageFormat, now.ToString("o"), Country, Federation, source.Team.Name, source.Team.Id, source.Name, source.Id, messagePart);
            this.messageQueue.Enqueue(message);
        }

        public bool HasMore
        {
            get
            {
                return this.messageQueue.Count > 0;
            }
        }

        public string Dequeue()
        {
            string message;
            if(this.messageQueue.TryDequeue(out message))
            {
                return message;
            }
            return null;
        }
    }
}
