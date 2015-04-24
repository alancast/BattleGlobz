//----------------------------------------------------------------------------------
// Global script for window management. Can be abandoned if you have only one menu window
//----------------------------------------------------------------------------------

#pragma strict

var windows: MenuWindow[];		// List of all windows
var activeWindow: int;			// Start/current window index
var autoIndex: boolean = false; // All windows will be indexed automatically according to their  order in windows array
var defaultScreenSize: Vector2 = Vector2 (800,480); // Default size of Screen. Size of all windows (and their elements)
                                                    // will be adjusted according to it. IF windows autoAdjustSize = true 

private var screenSizeMultiplier: Vector2 = Vector2 (0,0);
private var actionToPerform: Action;
private var lastActive: int = -1;


//========================================================================================================
// Init all windows
function Start () 
{
  screenSizeMultiplier.x = Screen.width/defaultScreenSize.x;
  screenSizeMultiplier.y = Screen.height/defaultScreenSize.y;
  
  if (windows.Length>0)
    {
     for (var i=0; i<windows.length; i++)  
       {
        windows[i].SetParent(this);
        windows[i].enabled = false;
        windows[i].SetIndex(i);
        windows[i].SetScreenSizeMultiplier(screenSizeMultiplier);
       }
       
       windows[activeWindow].enabled = true;
    }
    
   // lastActive = activeWindow;
}

//----------------------------------------------------------------------------------
// Process actions sended by windows 
function Update ()
{
   
  if (actionToPerform!=Action.none)
  {
  
   if (windows.Length > 0)
    for (var i=0; i<windows.length; i++)  
       if (windows[i].GetAction()!=Action.none) 
          {
           actionToPerform =  windows[i].GetAction();
           windows[i].SetAction(Action.none);
           lastActive = i;
           break;
          }
          

     var WinParam: float;    
              
    switch (actionToPerform)
	  {
	    case Action.close:
	      windows[lastActive].enabled = false;
	    break;

	    case Action.close_GoToWindow:
          WinParam = windows[lastActive].GetActionParameter();
	      windows[lastActive].enabled = false;
	      if (windows.Length >= WinParam) 
	       {
	        windows[WinParam].enabled = true;
	        activeWindow = WinParam;
	       }
	    break;
	    
	    
	    case Action.GoToWindow:
	      WinParam = windows[lastActive].GetActionParameter();

	      if (windows.Length >= WinParam) 
	       {
	        windows[WinParam].enabled = true;
	        activeWindow = WinParam;
	       }
	    break;
	    
	    	    	    
	    case Action.close_GoToNextWindow:
	      windows[lastActive].enabled = false;
	      if (windows.Length >= lastActive+1) 
	       {
	        windows[lastActive+1].enabled = true;
	        activeWindow = lastActive+1;
	       }
	    break;
	    
	    case Action.close_GoToPreviousWindow:
	      windows[lastActive].enabled = false;
	      if (lastActive-1 >= 0) 
	       {
	        windows[lastActive-1].enabled = true;	 
	        activeWindow = lastActive-1;
	       }
	    break;
	    
	    case Action.GoToNextWindow:
	      if (windows.Length >= lastActive+1) 
	       {
	        windows[lastActive+1].enabled = true;
	        activeWindow = lastActive+1;
	       }
	    break;
	    
	    case Action.GoToPreviousWindow:
	      if (lastActive-1 >= 0) 
	       {
	        windows[lastActive-1].enabled = true;	 
	        activeWindow = lastActive-1;
	       }
	    break;
	    
	    
	    case Action.close_MenuManager:
	    
	      if (windows.Length>0)
		    for (i=0; i<windows.length; i++)  
			        windows[i].enabled = false;
			        
		   this.enabled = false;
		   
	    break;
     }
     
      actionToPerform = Action.none;
     
  }
  
}


function OnEnable () {
   if (windows[activeWindow]) windows[activeWindow].enabled = true;
}

function OnDisable () {
   if (windows[activeWindow]) windows[activeWindow].enabled = false;
}

//----------------------------------------------------------------------------------
// Functions  for set/get local action variable
function SetAction (action: Action) 
{
  actionToPerform = action;
}

function GetAction (): Action
{
  return actionToPerform;
}

//----------------------------------------------------------------------------------