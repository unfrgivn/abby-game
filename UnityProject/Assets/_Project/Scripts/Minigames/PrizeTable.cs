using System;
using System.Collections.Generic;
using UnityEngine;

namespace WildsOfCloverhollow.Minigames
{
    [CreateAssetMenu(fileName = "PrizeTable", menuName = "Wilds of Cloverhollow/Minigames/Prize Table")]
    public class PrizeTable : ScriptableObject
    {
        [Header("Best Tier (0-10% from center)")]
        [SerializeField] private List<PrizeEntry> bestTierPrizes = new List<PrizeEntry>();
        
        [Header("Good Tier (10-30% from center)")]
        [SerializeField] private List<PrizeEntry> goodTierPrizes = new List<PrizeEntry>();
        
        [Header("Medium Tier (30-60% from center)")]
        [SerializeField] private List<PrizeEntry> mediumTierPrizes = new List<PrizeEntry>();
        
        [Header("Low Tier (60-100% from center)")]
        [SerializeField] private List<PrizeEntry> lowTierPrizes = new List<PrizeEntry>();

        public PrizeEntry GetRandomPrize(PrizeTier tier)
        {
            var prizes = GetPrizesForTier(tier);
            if (prizes == null || prizes.Count == 0)
            {
                return new PrizeEntry { prizeType = PrizeType.Nothing, amount = 0, weight = 1 };
            }

            int totalWeight = 0;
            foreach (var prize in prizes)
            {
                totalWeight += prize.weight;
            }

            if (totalWeight <= 0)
            {
                return prizes[0];
            }

            int roll = UnityEngine.Random.Range(0, totalWeight);
            int cumulative = 0;
            
            foreach (var prize in prizes)
            {
                cumulative += prize.weight;
                if (roll < cumulative)
                {
                    return prize;
                }
            }

            return prizes[prizes.Count - 1];
        }

        private List<PrizeEntry> GetPrizesForTier(PrizeTier tier)
        {
            return tier switch
            {
                PrizeTier.Best => bestTierPrizes,
                PrizeTier.Good => goodTierPrizes,
                PrizeTier.Medium => mediumTierPrizes,
                PrizeTier.Low => lowTierPrizes,
                _ => lowTierPrizes
            };
        }

        public int GetPrizeAmount(PrizeEntry entry)
        {
            return entry.amount;
        }

        public static int GetDefaultAmount(PrizeType type)
        {
            return type switch
            {
                PrizeType.GemsSmall => 5,
                PrizeType.GemsMedium => 15,
                PrizeType.GemsLarge => 50,
                PrizeType.CandyBar1 => 1,
                PrizeType.CandyBar2 => 2,
                _ => 0
            };
        }
    }
}
