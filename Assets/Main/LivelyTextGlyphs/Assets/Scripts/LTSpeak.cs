using System.Collections;
using UnityEngine;

namespace LivelyTextGlyphs
{
    public class LTSpeak : MonoBehaviour
    {
		// text to place inside the reference TextComponent
        [Multiline]
        public string Text;

		// LTText component used to bind configured Text 
        public LTText TextComponent;

		// visual element to show between pages
        public MonoBehaviour ContinueVisual;

		// reference to the type component along with the LTText component
        GlyphAnimType typeComponent;

		// true if this should capture a mouse down
        bool captureMouse;

		// true if a mouse down event was captured during the last frame
        bool mouseDown;

        private void Awake()
        {
            typeComponent = GetComponent<GlyphAnimType>();
            if (!typeComponent)
                typeComponent = GetComponentInChildren<GlyphAnimType>();
        }

        private void OnEnable()
        {
            if (TextComponent)
                StartCoroutine("Output");
        }

        private void OnDisable()
        {
            if (TextComponent)
            {
                StopCoroutine("Output");
                if (typeComponent)
                    typeComponent.enabled = false;
            }
        }

        void Update()
        {
            if (captureMouse)
                mouseDown = Input.GetMouseButtonDown(0);
        }

        IEnumerator Output()
        {
            if (ContinueVisual)
                ContinueVisual.gameObject.SetActive(false);

            TextComponent.Text = "<anim=type>" + Text + "</anim>";
            TextComponent.PageNumber = 1;
            TextComponent.ForceRebuild();
            TextComponent.ForceNewPage();

            while (true)
            {
                yield return new WaitForSeconds(0.25f);

                // wait for end of typing
                if (typeComponent)
                    while (typeComponent.enabled)
                        yield return null;

                // finished?
                if (TextComponent.PageNumber == TextComponent.PageCount)
                    break;

                // show continue
                if (ContinueVisual)
                    ContinueVisual.gameObject.SetActive(true);

                // wait for mouse down to advance to next page
                captureMouse = true;
                while (!mouseDown)
                    yield return null;
                captureMouse = false;
                mouseDown = false;

                // remove continue visual
                if (ContinueVisual)
                    ContinueVisual.gameObject.SetActive(false);

                // advance to next page
                TextComponent.PageNumber = TextComponent.PageNumber + 1;
                TextComponent.ForceNewPage();
            }

            // remove continue visual
            if (ContinueVisual)
                ContinueVisual.gameObject.SetActive(false);

            enabled = false;
        }
    }
}