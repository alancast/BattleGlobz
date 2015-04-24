// Script should be assigned to active object
// If this object contains MenuWindow script - everything will work already. 
// Or you can control another object (that contains MenuWindow script) - it should be assigned to MenuObject. 
// This object should be active, but attached MenuWindow script should be disabled


#pragma strict

var MenuObject: MenuWindow;
// Use MenuManager type instead MenuWindow of if  you want  to operate  with whole menu system on scene
var buttonCode: KeyCode = KeyCode.Escape;

function Update () {
	  
	    if (Input.GetKeyUp(buttonCode))
		{
		   
		   if (MenuObject)
			    MenuObject.enabled = !MenuObject.enabled;
			   else
			    {
			      var Script: MenuWindow = gameObject.GetComponent(MenuWindow);
			      if (Script)  Script.enabled = !Script.enabled;
			        else
			          Debug.Log ("Sorry but there no MenuWindow script attached to current object and no assigned to ", MenuObject);
			    }
			  
			// if (Time.timeScale == 0) Time.timeScale = 1; else Time.timeScale = 0;
		}
}