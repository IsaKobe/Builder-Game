using UnityEngine;
using UnityEngine.EventSystems;

public class EventHandler : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] GameObject buildMenu;
    [SerializeField] Transform catogoryMenu;
    [SerializeField] int sensitivity = 100;

    // camera movement
    int x = 0;
    int y = 0;
    bool left = false;
    bool up = false;
    bool rotation = false;
    void Update()
    {
        GridTiles gt = transform.GetChild(0).GetComponent<GridTiles>();
        // toggle build menu
        if (Input.GetButtonDown("Build Menu"))
        {
            buildMenu.SetActive(!buildMenu.activeSelf);
            for (int i = catogoryMenu.childCount - 1; i <= 0; i++)
            {
                catogoryMenu.GetChild(i).gameObject.SetActive(buildMenu.activeSelf);
            }
            buildMenu.transform.GetChild(2).gameObject.SetActive(false);
        }
        // toggle dig
        if (Input.GetButtonDown("Dig"))
        {
            gt.ChangeSelMode(SelectionMode.dig);
            gt.Exit(gt.activeObject);
            gt.Enter(gt.activeObject);
        }
        // toggle deconstruct
        else if (Input.GetButtonDown("Deconstruct"))
        {
            gt.ChangeSelMode(SelectionMode.deconstruct);
            gt.Exit(gt.activeObject);
            gt.Enter(gt.activeObject);
        }
        // rotates buildign
        else if (Input.GetButtonDown("Build Rotate"))
        {
            float axis = Input.GetAxis("Build Rotate");
            if(MyGrid.gridTiles.selMode == SelectionMode.build)
            {
                if (MyGrid.gridTiles.buildBlueprint.GetComponent<Pipe>())
                    return;
                if (axis < 0)
                {
                    MyGrid.gridTiles.buildBlueprint.transform.Rotate(new Vector3(0, 90, 0));
                }
                else
                {
                    MyGrid.gridTiles.buildBlueprint.transform.Rotate(new Vector3(0, -90, 0));
                }
                MyGrid.gridTiles.Enter(MyGrid.gridTiles.activeObject);
            }
        }
        // opens ingame menu
        if (Input.GetButtonDown("Menu"))
        {
            GameObject menu = GameObject.Find("Ingame Menu").transform.GetChild(0).gameObject;
            if (menu.activeSelf)
            {
                GameObject.Find("Scene").GetComponent<Tick>().Unpause();
            }
            else
            {
                GameObject.Find("Scene").GetComponent<Tick>().ChangeGameSpeed(0);
            }
            menu.SetActive(!menu.activeSelf);
            menu.transform.parent.GetChild(2).gameObject.SetActive(menu.activeSelf);
            Camera.main.GetComponent<PhysicsRaycaster>().enabled = !menu.activeSelf;
            Camera.main.GetComponent<Physics2DRaycaster>().enabled = !menu.activeSelf;
        }

        if (Input.GetButtonDown("Shift"))
        {
            if(gt.selMode == SelectionMode.deconstruct)
            {
                gt.Enter(gt.activeObject);
            }
        }
        CameraMovement();
    }
    void CameraMovement()
    {
        // resets camera to world origin
        if (Input.GetButtonDown("Jump"))
        {
            Camera.main.transform.localPosition = new(0, 0, 0);
            Camera.main.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        MoveVert();
        MoveHor();
        Rotate();
    }
    void MoveVert()
    {
        if (Input.GetAxis("Vertical") > 0)
        {
            if (up == false)
            {
                y = 0;
                up = true;
            }
            else
            {
                y++;
                if (y > sensitivity)
                {
                    Camera.main.transform.Translate(0, 0.04f, 0);
                }
                Camera.main.transform.Translate(0, 0.02f, 0);
            }
        }
        if (Input.GetAxis("Vertical") < 0)
        {
            if (up == true)
            {
                y = 0;
                up = false;
            }
            else
            {
                y++;
                if (y > sensitivity)
                {
                    Camera.main.transform.Translate(0, -0.04f, 0);

                }
                else
                {
                    Camera.main.transform.Translate(0, -0.02f, 0);
                }
            }
        }
    }
    void MoveHor()
    {
        if (Input.GetAxis("Horizontal") > 0)
        {
            if (left == false)
            {
                x = 0;
                left = true;
            }
            else
            {
                x++;
                if (x > sensitivity)
                {
                    Camera.main.transform.Translate(0.04f, 0, 0);
                }
                else
                {
                    Camera.main.transform.Translate(0.02f, 0, 0);
                }
            }
        }
        if (Input.GetAxis("Horizontal") < 0)
        {
            if (left == true)
            {
                x = 0;
                left = false;
            }
            else
            {
                x++;
                if (x > sensitivity)
                {
                    Camera.main.transform.Translate(-0.04f, 0, 0);

                }
                else
                {
                    Camera.main.transform.Translate(-0.02f, 0, 0);
                }
            }
        }
    }
    void Rotate()
    {
        if(Input.GetButton("Camera Rotate")){
            rotation = true;
            Camera.main.transform.Rotate(new Vector3(0, 0, Input.GetAxis("Camera Rotate")));
            //print(Input.GetAxis("Camera Rotate"));
        }
        else if (rotation)
        {
            Input.ResetInputAxes();
            rotation = false;
        }
    }
}
