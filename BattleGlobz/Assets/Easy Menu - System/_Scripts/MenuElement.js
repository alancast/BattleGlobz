//----------------------------------------------------------------------------------
//  Atomic class of menu elements
//  All basic functionality integrated already
//----------------------------------------------------------------------------------

#pragma strict

class MenuElements
{

// Type of element determines it appearnce and functionality. You can extend it as you want
 public enum ElementTypes 
 { 
	 button_CloseGoTo,  // Create button that closes current menu window and opens window with index parameterFloat in MenuManager script
	 button_GoTo,       // Create button that opens window with index parameterFloat in MenuManager script
	 button_CloseBack,  // Create button that closes current menu window and opens window with previous index in MenuManager script
	 button_CloseNext,  // Create button that closes current menu window and opens window with next index in MenuManager script
	 button_Back, 		// Create button that opens window with previous index in MenuManager script
	 button_Next, 		// Create button that opens window with next index in MenuManager script
	 button_ExitGame,   // Create button that close application
	 button_LoadLevel,  // Create button that load level with index parameterFloat
	 button_SetQuality, // Create button that set quality level according to parameter (Fastest, Fast, ... Fantastic)
	 button_DecQuality, // Create button that decrease quality level 
	 button_IncQuality, // Create button that increase quality level  
	 scroll_Resolutions,// Create scroll with list of all avaiable resolutions. Click will change gurrent resolution to choosen one
	 toggle_Fullscreen, // Create toggle that turn on/off fullscreen mode
	 slider_MouseSens,  // Create slider that can be used for Mouse sensitivity adjustment
	 button_Resume,     // Create button that close current menu and set time-scale to 1
	 button_Restart,    // Create button that restart current level
	 button_OpenURL,
	 label,             // Create text label   
	 stars,
	 button_CloseEverything,  // Close/disable whole menu manager and all related menus. 
	 textArea,
	 image,
	 toggle_Sound,
	 slider_masterVolume 
 } 

 var caption: String;					// Displayed caption of element
 var icon: Texture;						// To show near/instead of caption
 var type: ElementTypes;				// Type of element 
 var size: Vector2;						// Element size
 var globalAligment: Aligments;			// Element aligment in parent space
 var position: Vector2;					// Determines element position if it isn't preset by globalAligment
 var startAnimation: StartAnimation;    // Determines element animation at first appearance
 var animationSpeed: float;				// Animation speed
 var skin: GUISkin;						// GUI skin, if it isn't  specified - will be used Skin of parent element
 var parameter: String;					// Additional string parameter, should be  specified  for  some  types of elements
 var parameterFloat: float;				// Additional float parameter, should be  specified  for  some  types of elements
 var locked: boolean = false;			// Lock element if true. I.e. player can't use it. If lockImage assigned - also draw it above
 var lockImage: Texture;				// Image that indicate locked element
 
 private var parentElement: MenuWindow;
 private var parentSize: Vector2;
 private var currentPosition: Vector2;
 private var scrollPosition : Vector2 = Vector2.zero;
 private var Fullscreen_toggleBool : boolean;
 private var MouseSens: float;
 private var initialAnimation: StartAnimation;
 private var beenClicked: boolean = false;
 private var masterVolume : AudioListener;
  
  
//========================================================================================================
function SetParent (parent: MenuWindow) 
{
  parentElement = parent;
}
 

//----------------------------------------------------------------------------------
// Preparing element to be animated on enabling
function PrepareAnimations () 
{
  initialAnimation = startAnimation;
  
  switch (initialAnimation)
  {
   case StartAnimation.none:
     currentPosition = position;
    break;
    
    case StartAnimation.move_from_left:
     currentPosition = Vector2(0-size.x, position.y);
    break;
    
    case StartAnimation.move_from_right:
     currentPosition = Vector2(parentSize.x+size.x, position.y);
    break;
    
    case StartAnimation.move_from_top:
     currentPosition = Vector2(position.x, 0-size.y);
    break;
    
    case StartAnimation.move_from_bottom:
     currentPosition = Vector2(position.x, parentSize.y+size.y);
    break;
  }
}
 

//----------------------------------------------------------------------------------
// Align element and  setup start position/animation
function Start (screenSizeMultiplier: Vector2) 
{

  parentSize = parentElement.size;
  
  if (screenSizeMultiplier != Vector2.zero)
   {
     size.x *= screenSizeMultiplier.x;
     size.y *= screenSizeMultiplier.y;
  
     position.x *= screenSizeMultiplier.x;
     position.y *= screenSizeMultiplier.y;
   }
  
  switch (globalAligment)
  {
    case Aligments.center_center: 
        position.x = (parentSize.x-size.x)/2;
        position.y = (parentSize.y-size.y)/2;
      break;
    case Aligments.center_up: 
        position.x = (parentSize.x-size.x)/2;
        position.y = 0;
      break;  
    case Aligments.center_down: 
        position.x = (parentSize.x-size.x)/2;
        position.y = parentSize.y-size.y;
      break;   
      
    case Aligments.left_center: 
        position.x = 0;
        position.y = (parentSize.y-size.y)/2;
      break;   
    case Aligments.left_up: 
        position.x = 0;
        position.y = 0;
      break; 
    case Aligments.left_down: 
        position.x = 0;
        position.y = parentSize.y-size.y;
      break; 
      
    case Aligments.right_center: 
        position.x = parentSize.x-size.x;
        position.y = (parentSize.y-size.y)/2;
      break;   
    case Aligments.right_up: 
        position.x = parentSize.x-size.x;
        position.y = 0;
      break; 
    case Aligments.right_down: 
        position.x = parentSize.x-size.x;
        position.y = parentSize.y-size.y;
      break; 
      
     case Aligments.left: 
        position.x = 0;
      break; 
    case Aligments.center_y: 
        position.x = (parentSize.x-size.y)/2;
        position.y = (parentSize.y-size.y)/2;
      break;   
    case Aligments.center_x: 
        position.x = (parentSize.x-size.x)/2;
      break;  
    case Aligments.right: 
        position.x = parentSize.x-size.x;
      break; 
    case Aligments.down: 
        position.y = parentSize.y-size.y;
      break; 
    case Aligments.up: 
        position.y = 0;
      break;           
  }


   PrepareAnimations ();
  
  masterVolume = GameObject.FindObjectOfType(AudioListener);
  
   animationSpeed *=100;
}

//----------------------------------------------------------------------------------
// Animate element if animation specified
function Update () 
{

  if (initialAnimation!=StartAnimation.none)
    switch (initialAnimation)
	  {
	    case StartAnimation.move_from_left:
	     currentPosition.x += Time.deltaTime * animationSpeed;
	     if  (currentPosition.x >= position.x)   
	       {
		       initialAnimation=StartAnimation.none;
		       currentPosition = position;
	       }
	    break;
	    
	    case StartAnimation.move_from_right:
	     currentPosition.x -= Time.deltaTime * animationSpeed;
	     if  (currentPosition.x <= position.x)   
	       {
		       initialAnimation=StartAnimation.none;
		       currentPosition = position;
	       }
	    break;
	    
	    case StartAnimation.move_from_top:
	     currentPosition.y += Time.deltaTime * animationSpeed;
	     if  (currentPosition.y >= position.y)   
	       {
		       initialAnimation=StartAnimation.none;
		       currentPosition = position;
	       }
	    break;
	    
	    case StartAnimation.move_from_bottom:
	     currentPosition.y -= Time.deltaTime * animationSpeed;
	     if  (currentPosition.y <= position.y)   
	       {
		       initialAnimation=StartAnimation.none;
		       currentPosition = position;
	       }
	    break;
	  }
}

//----------------------------------------------------------------------------------
// Draw and handle  element according to it type
function OnGUI ()
{
  
 if(skin) GUI.skin = skin;

  switch (type)
  {
  
   case ElementTypes.button_CloseGoTo: 
       if (GUI.Button (Rect (currentPosition.x, currentPosition.y, size.x, size.y), GUIContent (caption, icon)))
         if (!locked) parentElement.SetAction (Action.close_GoToWindow, parameterFloat);
          
      break;
      
   case ElementTypes.button_GoTo: 
       if (GUI.Button (Rect (currentPosition.x, currentPosition.y, size.x, size.y), GUIContent (caption, icon)))
        if (!locked) parentElement.SetAction (Action.GoToWindow, parameterFloat);
      break;
      
      
   case ElementTypes.button_Back: 
       if (GUI.Button (Rect (currentPosition.x, currentPosition.y, size.x, size.y), GUIContent (caption, icon)))
         if (!locked) parentElement.SetAction (Action.GoToPreviousWindow);
      break;
      
   case ElementTypes.button_Next: 
       if (GUI.Button (Rect (currentPosition.x, currentPosition.y, size.x, size.y), GUIContent (caption, icon)))
        if (!locked) parentElement.SetAction (Action.GoToNextWindow);
      break;
      
      
   case ElementTypes.button_CloseBack: 
       if (GUI.Button (Rect (currentPosition.x, currentPosition.y, size.x, size.y), GUIContent (caption, icon)))
         if (!locked) parentElement.SetAction (Action.close_GoToPreviousWindow);
      break;
      
   case ElementTypes.button_CloseNext: 
       if (GUI.Button (Rect (currentPosition.x, currentPosition.y, size.x, size.y), GUIContent (caption, icon)))
        if (!locked) parentElement.SetAction (Action.close_GoToNextWindow);
      break; 
      
      
   case ElementTypes.button_ExitGame: 
       if (GUI.Button (Rect (currentPosition.x, currentPosition.y, size.x, size.y), GUIContent (caption, icon)))
         if (!locked) Application.Quit();
      break; 

   case ElementTypes.button_LoadLevel: 
       if (GUI.Button (Rect (currentPosition.x, currentPosition.y, size.x, size.y), GUIContent (caption, icon)))
          if (!locked) 
            {
             if (parentElement.gameObject.GetComponent.<AudioSource>()) 
             {
              beenClicked = true;
              parentElement.gameObject.GetComponent.<AudioSource>().Play();
             }
              else
                Application.LoadLevel(parameterFloat);
            }
            
            if (parentElement)
             if (parentElement.gameObject.GetComponent.<AudioSource>() && beenClicked)  
                  if (!parentElement.gameObject.GetComponent.<AudioSource>().isPlaying) Application.LoadLevel(parameterFloat);
      break; 
      
      
   case ElementTypes.button_SetQuality: 
       if (GUI.Button (Rect (currentPosition.x, currentPosition.y, size.x, size.y), GUIContent (caption, icon)))
       if (!locked) 
        {
	      switch (parameter)
		  {
		    case "Fastest":
		     QualitySettings.SetQualityLevel(QualityLevel.Fastest);
		    break;
		    
		    case "Fast":
		     QualitySettings.SetQualityLevel(QualityLevel.Fast);
		    break;
		    
		    case "Simple":
		     QualitySettings.SetQualityLevel(QualityLevel.Simple);
		    break;
		    
		    case "Good":
		     QualitySettings.SetQualityLevel(QualityLevel.Good);
		    break;
		    
		    
		    case "Beautiful":
		     QualitySettings.SetQualityLevel(QualityLevel.Beautiful);
		    break;
		    
		    
		    case "Fantastic":
		     QualitySettings.SetQualityLevel(QualityLevel.Fantastic);
		    break;
		  }
		 if (parameterFloat != 0) parentElement.SetAction (Action.close_GoToPreviousWindow);
		 }
      break;  


   case ElementTypes.button_IncQuality: 
       if (GUI.Button (Rect (currentPosition.x, currentPosition.y, size.x, size.y), GUIContent (caption, icon)))
         if (!locked)  QualitySettings.IncreaseLevel();
      break; 
      
      
   case ElementTypes.button_DecQuality: 
       if (GUI.Button (Rect (currentPosition.x, currentPosition.y, size.x, size.y), GUIContent (caption, icon)))
        if (!locked) QualitySettings.DecreaseLevel();
      break; 
  
         
   case ElementTypes.scroll_Resolutions: 
       if (!locked) 
        {
         var resolutions  = Screen.resolutions;
		 scrollPosition  = GUI.BeginScrollView (Rect (currentPosition.x, currentPosition.y, size.x, size.y),
		 scrollPosition, Rect (0, 0, size.x*0.8, resolutions.Length*parameterFloat));
		 
		  var i: int = 0;
		    for (var res in resolutions) 
		    {
		      	if (GUI.RepeatButton (new Rect (0, i*parameterFloat, size.x*0.9, parameterFloat*1.1), res.width + "x" + res.height)) 
		      	  {
		        	  Screen.SetResolution (res.width, res.height, Screen.fullScreen);
		        	  Application.LoadLevel(Application.loadedLevel);
		      	  }
		
		      i++;
		    }
	 	 GUI.EndScrollView();
	 	}
      break; 
      
               
                        
   case ElementTypes.toggle_Fullscreen: 
         var  isFullScreen = Screen.fullScreen;
         Fullscreen_toggleBool  = GUI.Toggle (Rect (currentPosition.x, currentPosition.y, size.x, size.y), isFullScreen, GUIContent (caption, icon));
		  if (!locked) 
		   { if (Fullscreen_toggleBool != isFullScreen) Screen.fullScreen = Fullscreen_toggleBool;}
      break; 
      
          
   case ElementTypes.slider_MouseSens:                                                           
       GUI.Label (Rect (currentPosition.x, currentPosition.y, caption.Length*parameterFloat, size.y), GUIContent (caption, icon));
       if (!locked) MouseSens = GUI.HorizontalSlider(new Rect(currentPosition.x+caption.Length*parameterFloat, currentPosition.y, size.x - caption.Length*parameterFloat, size.y), MouseSens, 1, 100 );		
	 break;	                                                                                                                                                                                                                                                                                                                                        
    
   case ElementTypes.button_Resume: 
       if (GUI.Button (Rect (currentPosition.x, currentPosition.y, size.x, size.y), GUIContent (caption, icon)))
        if (!locked) 
          { 
            Time.timeScale = 1;
            parentElement.SetAction (Action.close);
          }
      break; 
      
   case ElementTypes.button_Restart: 
       if (GUI.Button (Rect (currentPosition.x, currentPosition.y, size.x, size.y), GUIContent (caption, icon)))
        if (!locked) 
          { 
            Time.timeScale = 1;
            Application.LoadLevel(Application.loadedLevel);
          }
      break; 
          
   case ElementTypes.button_OpenURL: 
       if (GUI.Button (Rect (currentPosition.x, currentPosition.y, size.x, size.y), GUIContent (caption, icon)))
        if (!locked) 
          { 
           Application.OpenURL(parameter);
          }
      break; 
      
                            
   case ElementTypes.label: 
         GUI.Label (Rect (currentPosition.x, currentPosition.y, size.x, size.y), GUIContent (caption + parameter, icon)); 
      break; 
  
  
   case ElementTypes.stars: 
         for (i = 0; i<parameterFloat; i++)
          {
              GUI.DrawTexture(Rect((currentPosition.x-(parameterFloat*size.x*1.1-size.x)/2)+i*size.x*1.1, currentPosition.y, size.x, size.y), icon, ScaleMode.StretchToFill, true);
          }
      break;                      

             
   case ElementTypes.button_CloseEverything: 
       if (GUI.Button (Rect (currentPosition.x, currentPosition.y, size.x, size.y), GUIContent (caption, icon)))
        if (!locked) 
          { 
             parentElement.SetAction (Action.close_MenuManager);
          }
      break;  
      
   case ElementTypes.textArea: 
        

	 scrollPosition  = GUI.BeginScrollView (Rect (currentPosition.x, currentPosition.y, size.x, size.y), scrollPosition, Rect (0, 0, size.x-18, size.y*parameterFloat));
		 var textAreaString = GUI.TextArea (new Rect (0, 0, size.x-15, size.y*(parameterFloat+1)), parameter);   
        //var textAreaString = GUI.TextArea (new Rect (currentPosition.x, currentPosition.y, size.x, size.y), parameter);   
 	 GUI.EndScrollView();
	    
        if (!locked) 
          { 
             parameter = textAreaString;
          }
          else
            {
              GUI.SetNextControlName ("none");
              GUI.FocusControl ("none");    
            }
      break; 
      
          
   case ElementTypes.image: 
          if (icon) GUI.DrawTexture(Rect (currentPosition.x, currentPosition.y, size.x, size.y), icon, ScaleMode.StretchToFill, true);
      break;    
           
           
   case ElementTypes.toggle_Sound: 
         var audioListener = GameObject.FindObjectOfType(AudioListener);
         var isSoundEnabled: boolean = GUI.Toggle (Rect (currentPosition.x, currentPosition.y, size.x, size.y), audioListener.enabled, GUIContent (caption, icon));
		 if (!locked)   audioListener.enabled = isSoundEnabled;
      break;
      
      
    case ElementTypes.slider_masterVolume:                                                           
       GUI.Label (Rect (currentPosition.x, currentPosition.y, caption.Length*parameterFloat, size.y), GUIContent (caption, icon));
       if (!locked)   
          masterVolume.volume  = GUI.HorizontalSlider(new Rect(currentPosition.x+caption.Length*parameterFloat, currentPosition.y, size.x - caption.Length*parameterFloat, size.y), masterVolume.volume , 0, 1 );		
	 break;	  
	                             
  }
  
  if (locked) 
      if (lockImage) GUI.DrawTexture(Rect (currentPosition.x, currentPosition.y, size.x, size.y), lockImage, ScaleMode.ScaleToFit, true);
 
}

//----------------------------------------------------------------------------------
// Lock or unlock element
function Locked (state: boolean)
{
  locked = state;
}

//----------------------------------------------------------------------------------
}