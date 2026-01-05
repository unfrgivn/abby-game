using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace WildsOfCloverhollow.Tests.PlayMode
{
    public class BootstrapSmokeTests
    {
        private const string BootstrapSceneName = "Bootstrap";
        private const string CloverHollowSceneName = "Cloverhollow";

        [UnityTest]
        public IEnumerator BootstrapScene_Loads_Successfully()
        {
            var loadOp = SceneManager.LoadSceneAsync(BootstrapSceneName, LoadSceneMode.Single);
            while (!loadOp.isDone)
            {
                yield return null;
            }

            var bootstrapScene = SceneManager.GetSceneByName(BootstrapSceneName);
            Assert.IsTrue(bootstrapScene.isLoaded, "Bootstrap scene should be loaded");
        }

        [UnityTest]
        public IEnumerator CloverHollow_LoadsAdditively_AfterBootstrap()
        {
            var bootstrapOp = SceneManager.LoadSceneAsync(BootstrapSceneName, LoadSceneMode.Single);
            while (!bootstrapOp.isDone)
            {
                yield return null;
            }

            var contentOp = SceneManager.LoadSceneAsync(CloverHollowSceneName, LoadSceneMode.Additive);
            while (!contentOp.isDone)
            {
                yield return null;
            }

            var cloverHollowScene = SceneManager.GetSceneByName(CloverHollowSceneName);
            Assert.IsTrue(cloverHollowScene.isLoaded, "Cloverhollow scene should be loaded additively");
        }

        [UnityTest]
        public IEnumerator SpawnAnchor_ExistsInCloverHollow()
        {
            var bootstrapOp = SceneManager.LoadSceneAsync(BootstrapSceneName, LoadSceneMode.Single);
            while (!bootstrapOp.isDone)
            {
                yield return null;
            }

            var contentOp = SceneManager.LoadSceneAsync(CloverHollowSceneName, LoadSceneMode.Additive);
            while (!contentOp.isDone)
            {
                yield return null;
            }

            yield return null;

            var spawnAnchor = Object.FindFirstObjectByType<World.SpawnAnchor>();
            Assert.IsNotNull(spawnAnchor, "SpawnAnchor should exist in Cloverhollow scene");
        }
    }
}
