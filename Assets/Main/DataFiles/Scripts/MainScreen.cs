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



        private int currentGroup;

        private int currentMission;



        private void Start()
        {


            currentMission = 0;
            currentGroup = 0;

            OpenCurrent();




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
            currentMission = mission;
            OpenCurrent();
        }


        private void OpenCurrent()
        {


            foreach (var g in groups)
            {
                g.SetActive(false);
            }
            groups[currentGroup].SetActive(true);
            if (currentGroup == 1)
            {
                foreach (var g in gameGroups)
                {
                    g.SetActive(false);
                }
                gameGroups[currentMission].SetActive(true);
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
