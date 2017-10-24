using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkInterface : MonoBehaviour
{
    public GUIStyle TextBoxStyle;

    //UI Class For Only Network 
    private class Button
    {
        private string name;
        private Rect rect;

        public Button(string name, float left, float top, float width, float height)
        {
            this.name = name;
            this.rect = new Rect(left, top, width, height);
        }

        ~Button()
        {
        }

        public void Draw(CallBack ToDo, ref bool flag)
        {
            if (flag && GUI.Button(this.rect, this.name))
            {
                ToDo();
                flag = false;
            }
        }
    }

    private class TextBox
    {
        private string text;
        private Rect rect;

        public TextBox(float left, float top, float width, float height)
        {
            this.text = "192.168.0.1";
            this.rect = new Rect(left, top, width, height);
        }

        ~TextBox()
        {
        }

        public void Draw(GUIStyle style, ref bool flag)
        {
            if (flag)
            {
                this.text = GUI.TextField(this.rect, this.text, style);
            }
        }

        public string Text
        {
            set { this.text = value; }
            get { return this.text; }
        }
    }


    //Button UIs for Network UI
    private bool ButtonUIFlag = true;
    private Button ServerButton = null; //= new Button("Server", new Vector2(0.25f, 0.5f), new Vector2());
    private Button ClientButton = null; //= new Button("Client", new Vector2(0.25f, 0.5f), new Vector2());

    private bool TextBoxUIFlag = false;
    private TextBox IPAddressInput = null;
    private TextBox IPAddressOutput = null;

    //delegate function for ui callback
    private delegate void CallBack();



    //intialized ui interface
    private void Awake()
    {
        float width = 300.0f;
        float height = 50.0f;

        ServerButton =
            new Button(
                "Server",
                0.25f * Screen.width - width * 0.5f,
                0.5f * Screen.height - height * 0.5f,
                width,
                height);

        ClientButton =
            new Button(
                "Client",
                0.75f * Screen.width - width * 0.5f,
                0.5f * Screen.height - height * 0.5f,
                width,
                height);

        IPAddressInput =
            new TextBox(
                0.25f * Screen.width - width * 0.5f,
                0.4f * Screen.height - height * 0.5f,
                0.5f * Screen.width + width * 1.0f,
                height);
        
        IPAddressOutput =
            new TextBox(
                0.25f * Screen.width - width * 0.5f,
                0.4f * Screen.height - height * 0.5f,
                0.5f * Screen.width + width * 1.0f,
                height);
    }

    // Use this for initialization
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {

    }

    //if you click Server Button, it would be run.
    private void onServerButton()
    {
        Debug.Log("Clicked Server Button");
        TextBoxUIFlag = true;
        CoSpatial.CreateServer();
        IPAddressOutput.Text = CoSpatial.Network.IP;

        CoSpatial.Network.RegisterHandler(CoSpatialProtocol.Type.Connected, (netMsg) => {
            Debug.Log("Client Connected And Remove IP Address Box");
            this.TextBoxUIFlag = false;
        });
    }

    //if you click Client Button, it would be run.
    private void onClientButton()
    {
        Debug.Log("Clicked Client Button");
        CoSpatial.CreateClient(IPAddressInput.Text);
    }

    private void OnGUI()
    {
        if (ServerButton != null) ServerButton.Draw(onServerButton, ref ButtonUIFlag);
        if (ClientButton != null) ClientButton.Draw(onClientButton, ref ButtonUIFlag);

        if (IPAddressInput != null) IPAddressInput.Draw(TextBoxStyle, ref ButtonUIFlag);
        if (IPAddressOutput != null) IPAddressOutput.Draw(TextBoxStyle, ref TextBoxUIFlag);
    }

}
