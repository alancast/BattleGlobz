//----------------------------------------------------------------------------------
// To levelWindow should be attached window with levels (SelectLevel prefab)
// It's highly recommend to have all elements(level buttons)in levelWindow to have arranged indexes (i.e. 1st level has index 0, 2nd - index 1, etc)
//----------------------------------------------------------------------------------

#pragma strict

var levelWindow: MenuWindow;

private var levelsNum: int;

function Update () 
{
    if (Input.GetKey("p")) 
     PlayerPrefs.DeleteAll();
}

//----------------------------------------------------------------------------------
//Will unlock level with index stored in "UnlockLevel" every time when scene will be loaded or object activated
function Start () 
{
   if (!levelWindow) levelWindow = this.GetComponent(MenuWindow);
   
   if (levelWindow)
     for (var i=0; i<levelWindow.Elements.Length; i++)
       if (PlayerPrefs.HasKey("UnlockLevel"+i.ToString())) levelWindow.Elements[i].Locked(false);
      
}

//----------------------------------------------------------------------------------
//Use this function to unlock level next to current(completed)
function CompleteLevel (index: int) 
{
  if (levelWindow)
    if (levelWindow.Elements.Length>(index+1))
      levelWindow.Elements[index+1].Locked(false);
      
  PlayerPrefs.SetInt("UnlockLevel"+(index+1).ToString(), 1);
}

//----------------------------------------------------------------------------------
//Use this function to level with specified index
function UnlockLevel (index: int) 
{
  if (levelWindow)
    if (levelWindow.Elements.Length>index)
      levelWindow.Elements[index].Locked(false);
      
  PlayerPrefs.SetInt("UnlockLevel"+index.ToString(), 1);
}
//----------------------------------------------------------------------------------