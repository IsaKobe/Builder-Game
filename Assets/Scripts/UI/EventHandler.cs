using UnityEngine;

public class EventHandler : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] GameObject BuildMenu;
    [SerializeField] Transform CatogoryMenu;
    [SerializeField] int sensitivity = 100;

    // camera movement
    int x = 0;
    int y = 0;
    bool left = false;
    bool up = false;

    void Update()
    {
        if (Input.GetButtonDown("Build Menu"))
        {
            BuildMenu.SetActive(!BuildMenu.activeSelf);
            for (int i = CatogoryMenu.childCount - 1; i <= 0; i++)
            {
                CatogoryMenu.GetChild(i).gameObject.SetActive(BuildMenu.activeSelf);
            }
        }
        /*if (Input.GetButtonDown(""))
        {
            BuildMenu.SetActive(!BuildMenu.activeSelf);
            for (int i = CatogoryMenu.childCount - 1; i <= 0; i++)
            {
                CatogoryMenu.GetChild(0).GetChild(i).gameObject.SetActive(false);
            }
        }*/
        cameraMovement();
    }
    void cameraMovement()
    {
        moveVert();
        moveHor();
    }
    void moveVert()
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
    void moveHor()
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
}
