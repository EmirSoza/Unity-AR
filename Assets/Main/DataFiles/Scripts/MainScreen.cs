using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
namespace Knife.HologramEffect
{

    public class MainScreen : MonoBehaviour
    {


        [SerializeField] private GameObjectsGroup[] groups;

        [SerializeField] private GameObjectsGroup[] gameGroups;

        [SerializeField] private GameObjectsGroup[] missions;


        public string[] answers;
        public string answer;
        private int currentGroup;
        private int currentGameGroup;
        private int currentMission;



        private void Start()
        {
            currentGameGroup = 0;
            currentMission = 0;
            currentGroup = 0;
            if (PlayerPrefs.HasKey("mission")) {
                currentMission = PlayerPrefs.GetInt("mission");
                currentGroup = PlayerPrefs.GetInt("group");
                currentGameGroup = PlayerPrefs.GetInt("gameGroup");
                
            }
           
                

                OpenCurrent();
          

        }

        private void OnApplicationQuit()
        {
            currentGameGroup = 0;
            currentMission = 0;
            currentGroup = 0;
            PlayerPrefs.DeleteAll();
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Next();

            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Previous();

            }
        }

        public void Lagre(string value)
        {
            PlayerPrefs.SetString("name", value);
        }


        private void Next()
        {



            if (currentGroup < (groups.Length - 1))
            {
                currentGroup++;

            }
            else if (currentGroup == (groups.Length - 1))
            {
                currentGroup = 0;
            }

            OpenCurrent();

        }

        private void Previous()
        {
            currentGroup--;
            if (currentGroup < 0)
            {
                currentGroup = groups.Length - 1;
            }

            OpenCurrent();
        }


        public void SetCurrentGroup(int current)
        {
            currentGroup = current;
            OpenCurrent();
        }

        public void SetCurrentMission(int mission)
        {
            if (mission < currentMission) {
                currentMission = mission;
                OpenCurrent();
            }
            else if (mission > currentMission && answers[currentMission] == answer)
            {
                currentMission = mission;
                OpenCurrent();
            }

            
        }

        public void ZeroizeNumbers()
        {
            SetCurrentGameGroup(0);
            SetCurrentGroup(0);
            SetCurrentMission(0);
        }
        public void SetAnswer(string a)
        {
            answer = a;
        }

        public void SetCurrentGameGroup(int gameGroup)
        {
            currentGameGroup = gameGroup;
            OpenCurrent();
        }


        private void OpenCurrent()
        {


            foreach (var g in groups)
            {
                g.SetActive(false);
            }
            groups[currentGroup].SetActive(true);
            PlayerPrefs.SetInt("group", currentGroup);

            if (currentGroup == 1)
            {
                foreach (var g in gameGroups)
                {
                    g.SetActive(false);
                }
                gameGroups[currentGameGroup].SetActive(true);
                PlayerPrefs.SetInt("gameGroup", currentGameGroup);


                if (currentGameGroup == 1)
                {
                    foreach (var g in missions)
                    {
                        g.SetActive(false);
                    }
                    missions[currentMission].SetActive(true);
                    PlayerPrefs.SetInt("mission", currentMission);

                }

            } 

        }


        [System.Serializable]
        private class GameObjectsGroup
        {
            [SerializeField] private GameObject[] gameObjects;

            public void SetActive(bool enabled)
            {
                foreach (var g in gameObjects)
                {
                    g.SetActive(enabled);
                }
            }
        }
    }
}
