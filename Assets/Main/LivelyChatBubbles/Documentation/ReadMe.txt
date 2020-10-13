
Price
Category: Editor Extensions/Game Toolkits

Title: Lively Chat Bubbles
Description:

<p>Lively Chat Bubbles is a complete system that allows you to create, configure and execute dynamic communication throughout a scene.  Very easy to use components and video tutorials show the depth and breadth of the system.</p>

<p>All of the hard work of placement, sizing, speed, audio and visibility have been implemented and documented in this easy to use package.</p>

<p><strong>FEATURES INCLUDE</strong></p>

<p><strong>Chat Bubble</strong><br>
- Rich text fully supported.<br>
- Auto resize based on the message content.<br>
- Fully customize the style, minimum sizing and wrapping width.<br>
- Configure each border allowing a dynamic extender to slide gracefully.</p>

<p><strong>Chat Anchor</strong><br>
- Default the radius and angle from the anchored transform for the bubble to be displayed.<br>
- Adjust the placement dynamically towards the center of the screen by a given percentage.<br>
- Enforce the bubbles to stay completely on the screen when the anchored transform nears the screen bounds.</p>

<p><strong>Chat Output</strong><br>
- Configure how fast text is output along with all characters that should delay the output slightly.</p>

<p><strong>Chat Mouthpiece</strong><br>
- Brings all of the above components together allowing developers to call a simple Speak() method passing a message.<br>
- Set the name, chat bubble prefab, chat anchor, chat output settings and audio clip to use when speaking.<br>
- Mouthpieces can be configured differently giving them a unique personality while speaking.</p>





Thank you for taking the time to purchase the Lively Chat Bubble toolkit.  Below are the list of videos and walkthroughs to give you a complete understanding of the system and how things relate.

Overview (Key Components)

	[ChatBubble] represents the UI control that is shown while speaking.
	[ChatAnchor] represents the point in 3D space that keeps the [ChatBubble] anchored each frame.
	[ChatOutputProfile] contains the timing information when text is spoken.
	[ChatMouthpiece] attaches to an NPC or Avatar and takes a references to all of the components above to give unique characteristics to the NPC's.  A Speak( message ) method can be called within code to execute a single message.  Speak() will only work if the [ChatMouthpiece] is not already speaking.
	[ChatZOrder] will reposition all chat bubbles from back/front multiple times a second.

Videos Demonstrations
[https://www.youtube.com/playlist?list=PLKwyKAw9h1du8mkYOGfDwuZD2isEaytc5]

	Avatar Quote Demonstration
	[https://youtu.be/cQ3ydp679gc]
	Part of the Lively Chat Bubble add-on for Unity.  Demonstrates how all of the components work together to create a rich chat bubble experience.  Clicking on an avatar will make them speak a random quote.
	
	Avatar Custom Message Demonstration
	[https://youtu.be/Cfpfh5MIqo4]
	Part of the Lively Chat Bubble add-on for Unity.  Demonstrates how a textbox can be used to force a random avatar to speak the custom message.
	
Video Walkthroughs
[https://www.youtube.com/playlist?list=PLKwyKAw9h1ds1AnN2VC1s84MH9a19hq-h]

	Chat Bubble Configuration
	[https://youtu.be/3GUUubE4g1U]
	
	Part 1/6 of the Lively Chat Bubble add-on for Unity.  Goes over the steps necessary to create and configure a chat bubble.
	
	1) Open up the Template scene provided or create a Canvas and set it to Screen Space Overlay.
	2) Drag/Drop the ChatBubbleTemplate prefab under the Canvas.
	3) Drag/Drop one of the BubbleFrame## sprites onto the ChatBubbleTemplate.Image.SourceImage property.  
	4) Expand the ChatBubbleTemplate and select the Extender component.
	5) Drag/Drop one of the BubbleFrame##Cap sprites onto the Extender.Image.SourceImage property.
	6) Select the ChatBubbleTemplate again.
	7) Configure all of the Extender properties.
	
		Border Info - Each border can be configured with a Margin (M), Near (N) and Far (F) offsets.  These properties are used to determine exactly where and how far along a given border the extender can slide.  Lines in the scene will appear to show you how the borders are currently configured.
		
		Dock - Tells the extender what border to dock to.
		Position - From 0 to 1 the current position of the extender.
		
		NOTE: The Dock/Position will be updated automatically when a ChatBubble is attached to a ChatAnchor.  Please see the Chat Anchor Walkthrough video for more information.
		
	8) Configure all of the Value properties.
	
		Name - Name that appears on the chat bubble.
		Message - Message that appears on the chat bubble.
		AutoSize - True if the chat bubble should automatically resize based on the message value.
		Wrap Width - Width in pixels until the chat bubble will automatically resize vertically.
		Minimum Size - Minimum size in pixels of the chat bubble when the message is too small to support it.
		
		NOTE: The Name/Message will be updated automatically when using a ChatMouthpiece component.  Please see the Chat Mouthpiece Walkthrough video for more information.
	
	Chat Anchor Walkthrough
	[https://youtu.be/36UJrKGOjGA]
	
	Part 2/6 of the Lively Chat Bubble add-on for Unity.  Goes over the steps necessary to create and configure a chat anchor.
	
	1) Make sure to go through all of the previous steps listed in the Chat Bubble Walkthrough video.
	2) Create a capsule and position it accordingly. (Represents an avatar)
	3) Create an empty game object under the capsule and position it slightly above the center.  
	4) Rename it to Anchor. (Represents the avatar's mouth)
	5) Add the Chat Anchor component to the Anchor game object.
	6) Drag/Drop the ChatBubbleTemplate created earlier to the ChatAnchor.AttachedBubble property.  
	
	NOTE: At this point the chat bubble will be repositioned by all of the Chat Anchor properties.
	
	7) Adjusted the rest of the Chat Anchor properties to your liking.
	
		Attached Radius - Radius in world units from the anchor transform to the bubble's pivot.
		Attached Angle - Angle in degrees around the transform to the bubble's pivot.
		Center Influence - Percentage to influence the bubble's angle towards the center of the screen.
		Tracking Speed - Smoothing speed as the bubble follows the anchor transform.
		Keep In View - True if the bubble should stay within the screen bounds until the anchor position is no longer visible.
	
	Chat Mouthpiece Walkthrough
	[https://youtu.be/IohyBRY-mRI]

	Part 3/6 of the Lively Chat Bubble add-on for Unity.  Goes over the steps necessary to create and configure a chat mouthpiece.
		
	1) Make sure to go through all of the previous steps listed in the Chat Anchor Walkthrough video.
	2) Add the Chat Mouthpiece component to the Capsule game component.
	3) Set the following properties:
	
		Canvas - Canvas where all chat bubble instances will be created.
		Chat Name - Name displayed when spoken.
		Chat Bubble Prefab - Prefab that is instantiated when spoken.
		Chat Output Profile - Chat output information used when spoken such as characters per second, etc.
		Chat Anchor - Chat anchor to attach all instantiated bubbles.
		Audio Source - Audio information to play while speaking.
		
		NOTE: The Chat Output Profile and Audio Source properties will be shown in more depth in the next 2 videos.
	
	4) Select the Canvas and Create an InputField game object.
	5) Anchor it to the bottom middle of the screen and set the width/position to your liking.
	6) Add a handler to the InputField.OnEndEdit event.
	7) Drag/Drop the Capsule game object to the OnEndEdit object property.
	8) Under the Function dropdown select [dynamic] ChatMouthpiece.Speak.
	
	NOTE: This will fire off the Speak method on the Chat Mouthpiece component passing in the text from the InputField.
	
	9) Play the scene and type in text.  Hit enter to force the Chat Mouthpiece to speak!
		
	Chat Output Walkthrough
	[https://youtu.be/YAvdCZd22fI]
	
	Part 4/6 of the Lively Chat Bubble add-on for Unity.  Goes over the steps necessary to create a custom chat output asset.
		
	1) Make sure to go through all of the previous steps listed in the Chat Mouthpiece Walkthrough video.
	2) Adjust the InputField to multiple lines and input a very large string.  This will help demonstrate the chat output settings more clearly.
	3) In the Data folder, right click and Create -> Chat -> Output Profile.
	4) Assign the new asset to the Capsule.ChatMouthpiece.ChatOutputProfile property.
	5) Adjust the new asset properties to your liking:
	
		Initial Delay - Time in seconds to wait before starting the chat message.
		Characters Per Second - How many characters per second are output to the chat bubble.
		Completion Click - True if the completion requires a mouse click or input key.
		Completion Delay - Time in seconds to wait after the completion of a chat message.
		Character for Long Delay - Characters that will force a delay time configured by the LongCharacterDelay.
		Long Character Delay - Time in seconds to wait after every outputted character configured by the CharactersForLongDelay.
		Characters for Short Delay - Characters that will force a delay time configured by the ShortCharacterDelay.
		Short Character Delay - Time in seconds to wait after every outputted character configured by the CharactersForShortDelay.

	NOTE: Changing the values at runtime will help to see the results immediately.
		
	Quotes and Sound Walkthrough
	[https://youtu.be/fICyeU67zEs]

	Part 5/6 of the Lively Chat Bubble add-on for Unity.  Goes over the steps necessary to create a custom quote list asset.  Also, how to attach and configure sound effects.
		
	1) Make sure to go through all of the previous steps listed in the prior walkthrough videos.
	2) Add the Avatar Quotes Handler component to the Capsule game object.
	3) Drag/Drop the QuotesDefault asset from the Data folder to the AvatarQuotesHandler.Quotes property.
	
	NOTE: Alternatively, you can create your own block of quotes by right clicking in the Data folder to Create -> Chat -> Avatar Quotes.
	
	4) Clicking on the Capsule will display a random quote in Play mode.
	5) Add an Audio Source component to the Capsule game object.
	6) Turn off PlayOnAwake and set the AudioClip property to the Sounds/chat clip.
	7) Drag/Drop the Audio Source component up to the ChatMouthpiece.AudioSource property.
	8) Clicking on the Capsule will now include sound when speaking.
	
	NOTE: You can adjust the AudioSource.Pitch property for a higher/lower spoken sound.
	
	
	Avatar Walkthrough
	[https://youtu.be/friPt38CTwE]
	
	Part 6/6 of the Lively Chat Bubble add-on for Unity.  Goes over the steps necessary to create multiple avatars using the components described throughout the prior walkthrough videos.
		
	1) Make sure to go through all of the previous steps listed in the prior walkthrough videos.
	2) Remove any Capsule or Chat Bubble game objects from the scene.
	3) Drag/Drop the Avatar prefab into the scene and adjust its position accordingly.
	4) Drag/Drop the Canvas to the Avatar.ChatMouthpiece.Canvas property.
	5) Duplicate the Avatar a few times and spread them out.
	6) Adjust each of their AudioSource.Pitch and ChatMouth.ChatName properties slightly for variations.
	7) Create a new game object and rename it to Navigation.  
	
	NOTE: This will be used as a zone for the avatars to move freely in.
	
	8) Add a Box Collider component to the Navigation game object and adjust its properties to match the video.
	9) With all Avatars selected, Drag/Drop the Navigation game object to the Avatar.MovementArea property.
	
	Play the scene and click on any of the avatars!
	
Additional Notes

	The videos forgot to mention how to configure the ChatZOrder component to reorder chat bubbles dynamically from back/front as their attached chat anchor reposition themselves over time.  
	
	Please use the following steps:
	
	1) Ensure that all avatars or game objects with Chat Anchor components exist under a single parent game object.
	2) On the parent game object add the ChatZOrder script.
	3) Chat bubbles will now be reordered multiple times a second as their attached anchors' distances change from the camera.
	
	
	
	
	
	
	
	
	



	
	
	














Chat Bubble

Keywords:
Key Images:
Audio/Video:
Screenshots:




The Chat UI & Speech Bubbles system pack provides the base means for easily adding a MMO-style in-game communications system (a scrolling rich-text chat box with support for many commands, and a speech bubbles system) to your game. The pack, which is designed for the new Unity UI (UGUI) focuses on providing the user interface and interaction components, and does so in a thoroughly-documented and extensible way, so that you can easily integrate it with your particular framework.

The behavior and look of the components can be modified easily, most of the time through the unity editor itself. Also, the full source code is provided, so that you can check how everything works and modify or extend it as you see fit. 

This package does not provide (and it is not intended to) the actual backend system for your chat. That is, it does not provide the chat channels, group channels or private messaging systems. It is designed to provide the user interface for those, which are most often very specific to the game. 



