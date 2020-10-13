using UnityEngine;

namespace LivelyTextGlyphs
{
    public class DemoHandler : MonoBehaviour
    {
        public LTSpeak SpeakControl;

		public Font Font0;
		public Font Font1;
		public Font Font2;
		public Font Font3;

        public void Start()
        {
            // set to first output
            OnValueChanged(0);
        }

        public void OnValueChanged(int index)
        {
            if (!SpeakControl)
                return;

            // stop current speak progress
            SpeakControl.enabled = false;
            SpeakControl.TextComponent.Alignment = TextAnchor.UpperLeft;

            switch (index)
            {
                case 0:
					// override font
					SpeakControl.TextComponent.Font = Font0;// AssetDatabase.LoadAssetAtPath<Font>("Assets/LivelyTextGlyphs/Assets/Fonts/Lato-Regular.ttf");
                    SpeakControl.TextComponent.FontSize = 26;
                    SpeakControl.TextComponent.LineSpacing = 1f;
                    SpeakControl.TextComponent.SetMaterialDirty();
                    // override text
                    SpeakControl.Text = @"" +
                        "<size=1.5><color=#44FF44,#FFFFFFAA>Lively Text Glyphs</color></size>\n" +
                        "\tWant to easily support tool-tips with embedded <icon=fire> icons? " +
                        "How about a need for <anim=wave>dynamic animations</anim> that are easily defined using built-in components? " +
                        "All of this and much <anim=shake><color=yellow>more</color></anim> are available with <anim=throb>Lively Text Glyphs</anim>.";
                    break;
                case 1:
					// override font
					SpeakControl.TextComponent.Font = Font1; // = AssetDatabase.LoadAssetAtPath<Font>("Assets/LivelyTextGlyphs/Assets/Fonts/OpenSans-Regular.ttf");
                    SpeakControl.TextComponent.FontSize = 24;
                    SpeakControl.TextComponent.LineSpacing = 1f;
                    SpeakControl.TextComponent.SetMaterialDirty();
                    // override text
                    SpeakControl.Text = @"" +
                        "<size=1.5><color=#44FF44,#FFFFFFAA>Fresh Familiarity</color></size>\n" +
                        "\tWith Lively Text Glyphs you can expect all of the familiar text block functionality related to characters, paragraphs, and rich-text.\n" +
                        "\tIt becomes <anim=throb>fresh</anim> with the added support of up to <b><color=#FF8888,#88FF88,#8888FF,#FF88FF>four corner colors</color></b>, icons <icon=gears>, <anim=wave>animations</anim>, and paging. " +
                        "\n<i>It looks like you found the next page.</i>\n\n\tPaging can be configured to break on certain characters. This is helpful if you want text to break between sentences rather than mid-sentence.";
                    break;
                case 2:
					// override font
					SpeakControl.TextComponent.Font = Font2; // = AssetDatabase.LoadAssetAtPath<Font>("Assets/LivelyTextGlyphs/Assets/Fonts/Yantramanav-Regular.ttf");
                    SpeakControl.TextComponent.FontSize = 30;
                    SpeakControl.TextComponent.LineSpacing = 1f;
                    SpeakControl.TextComponent.SetMaterialDirty();
                    // override text
                    SpeakControl.Text = @"" +
                        "<size=1.5><color=#44FF44,#FFFFFFAA>Icon Injection</color></size>\n" +
                        "\tSimply create and bind an icon atlas <icon=window1> and reference an icon by name using the icon tag as seen in the DemoHandler script.\n" +
                        "\tThey even work <anim=wave>inside animations <icon=monitor1>!</anim>";
                    break;
                case 3:
					// override font
					SpeakControl.TextComponent.Font = Font3; // = AssetDatabase.LoadAssetAtPath<Font>("Assets/LivelyTextGlyphs/Assets/Fonts/MiniPixel-Regular.ttf");
                    SpeakControl.TextComponent.FontSize = 32;
                    SpeakControl.TextComponent.LineSpacing = 1f;
                    SpeakControl.TextComponent.SetMaterialDirty();
                    // override text
                    SpeakControl.Text = @"" +
                        "<size=1.5><color=#44FF44,#FFFFFFAA>Custom Animations</color></size>\n" +
                        "\tAs you can see you can have <anim=throb>several animations going on at once</anim>. " +
                        "Multiple animation components can be attached to a single text control and <anim=wave>configured in the designer</anim> along with a name. " +
                        "Reference the name when using the <anim=shake><color=yellow>anim</color></anim> tag as seen in the <anim=wave2>DemoHandler</anim> script.";
                    break;
            }

            // start speaking
            SpeakControl.enabled = true;
        }
    }
}