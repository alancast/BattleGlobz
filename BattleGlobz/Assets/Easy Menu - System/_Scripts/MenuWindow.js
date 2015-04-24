//----------------------------------------------------------------------------------
// This is main script. Create window with specified parameters and  elements bucket
//----------------------------------------------------------------------------------

#pragma strict

// Make the script also execute in edit mode. It's better to comment this string before release
//@script ExecuteInEditMode()


// Aligment in global space
public enum Aligments { none, center_center, center_up, center_down, left_center, left_up, left_down, right_center, right_up, right_down, center_x, center_y, left, right, up, down} 
// Determines  animation at first appearance
public enum StartAnimation { none, move_from_right, move_from_left, move_from_top, move_from_bottom } 
// Basic actions related for windows
public enum Action 
	{ 
		none, 						// none
		close, 						// Close  current window
		close_GoToNextWindow,  		// Close current and open window with next index in MenuManager script
		close_GoToPreviousWindow, 	// Close current and open window with previous index in MenuManager script
		GoToNextWindow, 			// Open window with next index in MenuManager script
		GoToPreviousWindow, 		// Open window with previous index in MenuManager script
		GoToWindow, 				// Open window with parameterFloat index in MenuManager script
		close_GoToWindow,           // Close current and open window with parameterFloat index in MenuManager script
		close_MenuManager			// Close/disable whole menu manager and all related menus. 
	} 
 
 var caption: String;					// Displayed caption of element
 var size: Vector2;						// Size of Window
 var scrollViewSize: Vector2;			// Size of scollable area inside
 var position: Vector2;					// Determines element position if it isn't preset by globalAligment
 var globalAligment: Aligments;  		// DElement aligment in global screen space
 var startAnimation: StartAnimation;	// Determines window animation at first appearance
 var animationSpeed: float;				// Animation speed
 var skin: GUISkin;						// GUI skin, if it isn't  specified - will be used Skin of parent element or default
 var icon: Texture;						// To show near/instead of caption
 var draggable: boolean = false;		// Will be  window  dragable or not
 var autoAdjustSize: boolean = true;	// Size of window and all elements will be adjusted according to screen resolution
 var interactionSound : AudioClip;      // Plays this sound after any interaction (like button pressing) with any elements from Elements array
 										// Please ensure that AudioListener component is attached
 										
 var Elements: MenuElements[];			// Bunch of elements in this window

 
 
 // Usefull internal variables
 private var index: int;						// Local windows index. SHOULD BE UNIQUE!
 private var actionToPerform: Action;
 private var currentPosition: Vector2;
 private var parentElement: MenuManager;
 private var parameterFloat: float=-1;
 private var scrollPosition : Vector2 = Vector2.zero;
 private var windowRect : Rect;
 private var initialAnimation: StartAnimation;
 private var screenSizeMultiplier: Vector2 = Vector2 (0,0);
//========================================================================================================
function SetParent (parent: MenuManager) 
{
  parentElement = parent;
}

//----------------------------------------------------------------------------------
function SetIndex (newID: int) 
{
  index = newID;
}

//----------------------------------------------------------------------------------
function SetScreenSizeMultiplier (newScreenSizeMultiplier: Vector2) 
{
  if (autoAdjustSize) 
    {
      screenSizeMultiplier = newScreenSizeMultiplier;
      
	  if (screenSizeMultiplier != Vector2.zero)
	   {
	    size.x *= screenSizeMultiplier.x;
	    size.y *= screenSizeMultiplier.y;
	   }
    }
    
}
//----------------------------------------------------------------------------------
function Start () 
{
  yield WaitForEndOfFrame();

  
  if (Elements.Length>0)
   for (var i=0; i<Elements.length; i++)  
    {
      Elements[i].SetParent(this);
      Elements[i].Start(screenSizeMultiplier);
    }
    
    animationSpeed *=100;
}

//----------------------------------------------------------------------------------
// Align element and  setup start position/animation
function OnEnable () 
{
  initialAnimation = startAnimation;
  
 if (Elements.Length>0)
   for (var i=0; i<Elements.length; i++)  
    Elements[i].PrepareAnimations();
    
  switch (globalAligment)
  {
    case Aligments.center_center: 
        position.x = (Screen.width-size.x)/2;
        position.y = (Screen.height-size.y)/2;
      break;
      
    case Aligments.center_up: 
        position.x = (Screen.width-size.x)/2;
        position.y = 0;
      break;  
      
    case Aligments.center_down: 
        position.x = (Screen.width-size.x)/2;
        position.y = Screen.height-size.y;
      break;   
      
    case Aligments.left_center: 
        position.x = 0;
        position.y = (Screen.height-size.y)/2;
      break;   
      
    case Aligments.left_up: 
        position.x = 0;
        position.y = 0;
      break; 
      
    case Aligments.left_down: 
        position.x = 0;
        position.y = Screen.height-size.y;
      break; 
      
    case Aligments.right_center: 
        position.x = Screen.width-size.x;
        position.y = (Screen.height-size.y)/2;
      break;   
      
    case Aligments.right_up: 
        position.x = Screen.width-size.x;
        position.y = 0;
      break; 
      
    case Aligments.right_down: 
        position.x = Screen.width-size.x;
        position.y = Screen.height-size.y;
      break; 
      
     case Aligments.left: 
        position.x = 0;
      break; 
      
    case Aligments.center_y: 
        position.x = (Screen.height-size.y)/2;
        position.y = (Screen.height-size.y)/2;
      break;   
      
    case Aligments.center_x: 
        position.x = (Screen.width-size.x)/2;
      break;  
      
    case Aligments.right: 
        position.x = Screen.width-size.x;
      break; 
      
    case Aligments.down: 
        position.y = Screen.height-size.y;
      break; 
      
    case Aligments.up: 
        position.y = 0;
      break;       
  }
 
   
  switch (initialAnimation)
  {
   case StartAnimation.none:
     currentPosition = position;
    break;
    
    case StartAnimation.move_from_left:
     currentPosition = Vector2(0-size.x, position.y);
    break;
    
    case StartAnimation.move_from_right:
     currentPosition = Vector2(Screen.width+size.x, position.y);
    break;
    
    case StartAnimation.move_from_top:
     currentPosition = Vector2(position.x, 0-size.y);
    break;
    
    case StartAnimation.move_from_bottom:
     currentPosition = Vector2(position.x, Screen.height+size.y);
    break;
  }
    
     
   if (scrollViewSize == Vector2.zero)   scrollViewSize = size;
   
   windowRect = Rect( currentPosition.x, currentPosition.y, size.x, size.y) ;
   
   

}


//----------------------------------------------------------------------------------
// Animate element if animation specified
function Update () 
{

  if (initialAnimation!=StartAnimation.none)
   {
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
	    windowRect = Rect( currentPosition.x, currentPosition.y, size.x, size.y) ;
	}
	
	
   if (Elements.Length>0)
     for (var i=0; i<Elements.length; i++) Elements[i].Update();
  
   if (actionToPerform!=Action.none) 
          if(parentElement) 
            {
              parentElement.SetAction(actionToPerform);
              //actionToPerform = Action.none;
            }
          
   if (actionToPerform == Action.close) 
          if(!parentElement) 
             {
              this.enabled = false;
              actionToPerform = Action.none;
             }
          

}
//----------------------------------------------------------------------------------
// Set window  action and/or paramenter for it
function SetAction (action: Action, param: float)
{
  actionToPerform = action;
  parameterFloat = param;
  if (GetComponent.<AudioSource>()) GetComponent.<AudioSource>().PlayOneShot(interactionSound);
     else Debug.Log ("If you want to have  sound feedback - Please ensure that AudioListener component is attached to " + gameObject.name);
}

function SetAction (action: Action)
{
  actionToPerform = action;
  if (GetComponent.<AudioSource>()) GetComponent.<AudioSource>().PlayOneShot(interactionSound);
}

//----------------------------------------------------------------------------------
// Get window  action and/or paramenter for it
function GetAction (): Action
{
  return actionToPerform;
}

function GetActionParameter (): float
{
  return parameterFloat;
}

//----------------------------------------------------------------------------------
// Draw window
function OnGUI () 
{
  if(skin) GUI.skin = skin;

  windowRect = GUI.Window (index, windowRect, WindowFunction, caption);

}

//----------------------------------------------------------------------------------
// Process all elements inside window
function WindowFunction (windowID : int) 
{
    if (icon) GUI.DrawTexture(Rect (0, 0, 30, 30), icon, ScaleMode.ScaleToFit, true);
     
    scrollPosition = GUI.BeginScrollView (Rect (0, 0, size.x, size.y), scrollPosition, Rect (0, 0, scrollViewSize.x, scrollViewSize.y));
    
     if (Elements.Length>0)
      for (var i=0; i<Elements.length; i++) Elements[i].OnGUI();

     GUI.EndScrollView ();
   
    // Make the windows be draggable.
    if (draggable)  GUI.DragWindow ();         
 }

//----------------------------------------------------------------------------------