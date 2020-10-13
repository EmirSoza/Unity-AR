using UnityEngine;

namespace Knife.HologramEffect
{
    public class MainScreen : MonoBehaviour
    {

      
        [SerializeField] private GameObjectsGroup[] groups;
       

   

        public int currentGroup;
       

        


        private void Start()
        {
           
            
            
            currentGroup = 0;
           
            OpenCurrent();

        
         
            
        }

       

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.RightArrow))
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
          


            if (currentGroup < (groups.Length -1))
                {
                currentGroup++;
                
                }
            else if (currentGroup == (groups.Length-1)) { currentGroup = 0;
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

        private void OpenCurrent()
        {
            
           
            foreach (var g in groups)
            {
                g.SetActive(false);
            }
            groups[currentGroup].SetActive(true);
        }
        

        [System.Serializable]
        private class GameObjectsGroup
        {
            [SerializeField] private GameObject[] gameObjects;

            public void SetActive(bool enabled)
            {
                foreach(var g in gameObjects)
                {
                    g.SetActive(enabled);
                }
            }
        }
    }
}