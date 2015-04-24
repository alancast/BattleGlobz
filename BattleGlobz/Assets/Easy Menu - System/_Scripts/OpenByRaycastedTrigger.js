// Script should be assigned to active object
// If this object contains MenuWindow script - everything will work already. 
// Or you can control another object (that contains MenuWindow script) - it should be assigned to MenuObject. 
// This object should be active, but attached MenuWindow script should be disabled


#pragma strict
// Use MenuManager type instead MenuWindow of if  you want  to operate  with whole menu system on scene

var MenuObject: MenuWindow;  
var TriggerObject: GameObject;

//=================================================================
function Update () 
{
	  
	    if (Input.GetMouseButtonDown(0)) RaycastTrigger ();
	
}

//-----------------------------------------------------------------
function RaycastTrigger ()
{
		var castRay: Ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		castRay.direction = castRay.direction.normalized;
			      
		 var hitInfo : RaycastHit;
		 
			      if (Physics.Raycast( castRay, hitInfo, Mathf.Infinity)) 
			            {
			              if(hitInfo.collider.gameObject == TriggerObject) OpenCloseMenu ();
			               Debug.DrawRay(castRay.origin, castRay.direction*100, Color.red);
						}
						
}

//-----------------------------------------------------------------						
function OpenCloseMenu () {
	   
		   if (MenuObject)
			    MenuObject.enabled = !MenuObject.enabled;
			   else
			    {
			      var pauseScript: MenuWindow = gameObject.GetComponent(MenuWindow);
			      if (pauseScript)  pauseScript.enabled = !pauseScript.enabled;
			        else
			          Debug.Log ("Sorry but there is no MenuWindow script attached to current object and no assigned to ", MenuObject);
			    }
			  
	
}
//-----------------------------------------------------------------