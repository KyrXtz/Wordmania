namespace Gravitons.UI.Modal
{
    using UnityEngine;
    using UnityEngine.UI;

    public class GenericModal : Modal
    {
        [Tooltip("Modal title")]
        [SerializeField] protected Text m_Title;
        [Tooltip("Modal body")]
        [SerializeField] protected Text m_Body;
        [SerializeField] protected Image m_Image;
        [Tooltip("Buttons in the modal")]
        [SerializeField] protected Button[] m_Buttons;

        /// <summary>
        /// Deactivate buttons in awake
        /// </summary>
        public void Awake()
        {
            for (int i = 0; i < m_Buttons.Length; i++)
            {
                m_Buttons[i].gameObject.SetActive(false);
            }
        }

        public override void Show(ModalContentBase modalContent, ModalButton[] modalButton)
        {
            GenericModalContent content = (GenericModalContent) modalContent;
            m_Title.text = content.Title;
            m_Body.text = content.Body;
            m_Image.sprite = content.Sprite;
            //Activate buttons and populate properties
            if (modalButton != null)
            {
                for (int i = 0; i < modalButton.Length; i++)
                {
                    if (i >= m_Buttons.Length)
                    {
                        Debug.LogError($"Maximum number of buttons of this modal is {m_Buttons.Length}. But {modalButton.Length} ModalButton was given. To display all buttons increase the size of the button array to at least {modalButton.Length}");
                        return;
                    }
                    m_Buttons[i].gameObject.SetActive(true);
                    m_Buttons[i].GetComponentInChildren<Text>().text = modalButton[i].Text;
                    int index = i; //Closure
                    m_Buttons[i].onClick.AddListener(() =>
                    {
                        if (modalButton[index].Callback != null)
                        {
                            modalButton[index].Callback();
                        }
                        if (modalButton[index].Callback2 != null)
                        {
                            modalButton[index].Callback2(modalButton[index].list);
                        }
                        if (modalButton[index].CloseModalOnClick)
                        {
                            Close();
                        }
                        m_Buttons[index].onClick.RemoveAllListeners();
                    });
                }
            }
        }
    }
}