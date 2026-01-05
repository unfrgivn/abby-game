using NUnit.Framework;
using UnityEngine;
using WildsOfCloverhollow.Minigames;

namespace WildsOfCloverhollow.Tests
{
    public class ClawMachineTests
    {
        private PrizeTable CreateTestPrizeTable()
        {
            var table = ScriptableObject.CreateInstance<PrizeTable>();
            return table;
        }

        private ClawMachineTuning CreateTestTuning()
        {
            var tuning = ScriptableObject.CreateInstance<ClawMachineTuning>();
            tuning.bestZonePercent = 0.1f;
            tuning.goodZonePercent = 0.3f;
            tuning.mediumZonePercent = 0.6f;
            return tuning;
        }

        [Test]
        public void GetTierForPosition_CenterPosition_ReturnsBestTier()
        {
            var tuning = CreateTestTuning();
            
            var tier = tuning.GetTierForPosition(0f);
            
            Assert.AreEqual(PrizeTier.Best, tier);
        }

        [Test]
        public void GetTierForPosition_NearCenter_ReturnsBestTier()
        {
            var tuning = CreateTestTuning();
            
            var tier = tuning.GetTierForPosition(0.05f);
            
            Assert.AreEqual(PrizeTier.Best, tier);
        }

        [Test]
        public void GetTierForPosition_NearCenterNegative_ReturnsBestTier()
        {
            var tuning = CreateTestTuning();
            
            var tier = tuning.GetTierForPosition(-0.05f);
            
            Assert.AreEqual(PrizeTier.Best, tier);
        }

        [Test]
        public void GetTierForPosition_GoodZone_ReturnsGoodTier()
        {
            var tuning = CreateTestTuning();
            
            var tier = tuning.GetTierForPosition(0.2f);
            
            Assert.AreEqual(PrizeTier.Good, tier);
        }

        [Test]
        public void GetTierForPosition_MediumZone_ReturnsMediumTier()
        {
            var tuning = CreateTestTuning();
            
            var tier = tuning.GetTierForPosition(0.45f);
            
            Assert.AreEqual(PrizeTier.Medium, tier);
        }

        [Test]
        public void GetTierForPosition_FarFromCenter_ReturnsLowTier()
        {
            var tuning = CreateTestTuning();
            
            var tier = tuning.GetTierForPosition(0.8f);
            
            Assert.AreEqual(PrizeTier.Low, tier);
        }

        [Test]
        public void GetTierForPosition_EdgeOfScreen_ReturnsLowTier()
        {
            var tuning = CreateTestTuning();
            
            var tier = tuning.GetTierForPosition(1f);
            
            Assert.AreEqual(PrizeTier.Low, tier);
        }

        [Test]
        public void GetTierForPosition_NegativeEdge_ReturnsLowTier()
        {
            var tuning = CreateTestTuning();
            
            var tier = tuning.GetTierForPosition(-1f);
            
            Assert.AreEqual(PrizeTier.Low, tier);
        }

        [Test]
        public void GetRandomPrize_EmptyTable_ReturnsNothingPrize()
        {
            var table = CreateTestPrizeTable();
            
            var prize = table.GetRandomPrize(PrizeTier.Best);
            
            Assert.AreEqual(PrizeType.Nothing, prize.prizeType);
        }

        [Test]
        public void GetDefaultAmount_GemsSmall_Returns5()
        {
            var amount = PrizeTable.GetDefaultAmount(PrizeType.GemsSmall);
            
            Assert.AreEqual(5, amount);
        }

        [Test]
        public void GetDefaultAmount_GemsMedium_Returns15()
        {
            var amount = PrizeTable.GetDefaultAmount(PrizeType.GemsMedium);
            
            Assert.AreEqual(15, amount);
        }

        [Test]
        public void GetDefaultAmount_GemsLarge_Returns50()
        {
            var amount = PrizeTable.GetDefaultAmount(PrizeType.GemsLarge);
            
            Assert.AreEqual(50, amount);
        }

        [Test]
        public void GetDefaultAmount_CandyBar1_Returns1()
        {
            var amount = PrizeTable.GetDefaultAmount(PrizeType.CandyBar1);
            
            Assert.AreEqual(1, amount);
        }

        [Test]
        public void GetDefaultAmount_CandyBar2_Returns2()
        {
            var amount = PrizeTable.GetDefaultAmount(PrizeType.CandyBar2);
            
            Assert.AreEqual(2, amount);
        }

        [Test]
        public void GetDefaultAmount_Nothing_Returns0()
        {
            var amount = PrizeTable.GetDefaultAmount(PrizeType.Nothing);
            
            Assert.AreEqual(0, amount);
        }
    }
}
