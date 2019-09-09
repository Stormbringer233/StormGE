using MyGE;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MyTemplate
{
    public static class ScenesManager
        /* Static game scene manage
         * This Class manage all of the master scenes in game
         * 
         * M. Le Thiec
         * 10/03/2019
         * 
         * V : 2.00
         * 
         * rev : 10/03/19 - change class from classic class to static
         * 
         */
    {
        public static Dictionary<string, Scene> ScenesList;
        public static Scene CurrentScene { get; private set; }
        public static bool SceneHasChange { get; set; }

        static Scene SavedScene; // allow to save a scene to restore it in the same state later

        public static void Initialize()
        {
            ScenesList = new Dictionary<string, Scene>();
        }

        public static void Add(string pName, Scene pScene)
        {
            // to prevent conflic, all scenes names are convert to uppercase
            pName = pName.ToUpper();
            ScenesList.Add(pName, pScene);
            pScene.SceneName = pName;
            pScene.SwitchScene += OnSwitchTo;
            //Console.WriteLine("Scene " + pName + " was add to the list of Scenes");
        }

        public static bool Remove(string pName)
            // simply remove a scene from list of scenes
        {
            if (ScenesList.ContainsKey(pName))
            {
                ScenesList.Remove(pName);
                return true;
            }
            return false;
        }

        public static Scene GetScene(string pName)
            // Return Scene object corresponding to Scene Name
        {
            pName = pName.ToUpper();
            //foreach(KeyValuePair<string, Scene> scene in ScenesList)
            //{
            //    Console.WriteLine("Scenes names : "+scene.Key+ " | " + scene.Value);
            //}
            if (ScenesList.ContainsKey(pName))
                return ScenesList[pName];
            return null;
        }

        public static string GetSceneName()
        {
            return CurrentScene.SceneName;
        }

        public static void Push(Scene pScene)
            // Save the scene to allow to restore it in same state later
        {
            if (SavedScene == null)
            {
                SavedScene = pScene;
            }
        }

        public static Scene Pop()
            //Restore the saved scene if exist
        {
            if (SavedScene != null)
            {
                Scene tempScene = SavedScene;
                SavedScene = null;
                return tempScene;
            }
            return null;
        }

        public static void OnSwitchTo(object sender, EventArgs e, string pToScene)
        {
            Debug.WriteLine("Scene - " + sender + " - has request a change to < "+pToScene+" > scene");
            SwitchTo(pToScene);
        }

        public static void SwitchTo(string pNewScene)
            // Switch to a new scene if exist
        {
            // uppercase scene name to corresponding to scene name in memory
            pNewScene = pNewScene.ToUpper();

            // Check if a scene is push. Switch to scene must be equivalent to pop it
            // Usefull in case of need to switch between 2 scenes without reinitialize it
            if (SavedScene != null)
            {
                //Restore the saved scene and don't call the initialize function
                if (SavedScene.SceneName != pNewScene)
                {
                    CurrentScene = ScenesList[pNewScene];
                    CurrentScene.LoadContent();
                    CurrentScene.SceneName = pNewScene;
                    CurrentScene.Restore();
                    SceneHasChange = true;
                    Pop(); // set SavedScene to null
                    Console.WriteLine("Scene " + pNewScene + " is now restored");
                }
            }
            else
            {
                if (CurrentScene != null)
                {
                    CurrentScene.UnloadContent();
                    CurrentScene = null;
                }

                if (ScenesList.ContainsKey(pNewScene))
                {
                    //Console.WriteLine("SwitchTo() - Now change scene to " + pNewScene);
                    CurrentScene = ScenesList[pNewScene];
                    CurrentScene.LoadContent();
                    CurrentScene.SceneName = pNewScene;
                    CurrentScene.Setup();
                    SceneHasChange = true;
                    Console.WriteLine("SwitchTo() - New scene is now : " + CurrentScene.SceneName + " | Scene Object : " + CurrentScene.ToString());
                }
            }
        }

        public static void Update(GameTime gameTime)
        {
            //Console.WriteLine("\t>> Entering SceneManager.Update()");
            CurrentScene.Update(gameTime);
            //Console.WriteLine("\t<< Finish SceneManager.Update()");
        }

        public static void Draw(GameTime gameTime)
        {
            CurrentScene.Draw(gameTime);
        }
    }
}
