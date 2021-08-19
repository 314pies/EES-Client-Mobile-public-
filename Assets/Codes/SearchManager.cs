using EES.ClientAPIs;
using EES.ClientAPIs.ClientModules;
using EES.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchManager : MonoBehaviour
{

    private static SearchManager instance;
    public static SearchManager Singleton
    {
        get
        {
            return instance;
        }
    }

    private const string SearchHistoryFileName = "History";
    public enum SearchMode
    {
        Search,
        Product,
        Booth,
        Exhibition
    }

    public SearchMode currentMode;
    public bool changedMode = false;
    private List<string> searchHistoryList;
    private List<Product> searchedProduct;
    private List<Booth> searchedBooth;
    private List<Exhibition> searchedExhibition;

    public Color Selected;
    public Color UnSelected;
    public Color UnSelectedText;

    public Image ProductButton;
    public Image BoothButton;
    public Image ExhibitionButton;

    public Text ResultText;

    public InputField Input;

    public Transform HistoryRoot;
    public Transform ProductRoot;
    public Transform BoothRoot;
    public Transform ExhibitionRoot;
    public GameObject SearchHistoryPrefab;
    public GameObject SearchProductPrefab;
    public GameObject SearchBoothPrefab;
    public GameObject SearchExhibitionPrefab;

    public GameObject HistoryScrollPanel;
    public GameObject ProductScrollPanel;
    public GameObject BoothScrollPanel;
    public GameObject ExhibitionScrollPanel;
    public GameObject NoResult;

    public GameObject ExhibitionDetails;
    public GameObject BoothDetails;

    public GameObject ClearAll;
    public GameObject ClearResult;

    void Start()
    {
        instance = this; 
        Initialize();
        gameObject.SetActive(false);
    }

    public void Initialize()
    {
        currentMode = SearchMode.Search;
        changedMode = false;

        searchedProduct = new List<Product>();
        searchedBooth = new List<Booth>();
        searchedExhibition = new List<Exhibition>();

        HistoryScrollPanel.SetActive(true);
        ProductScrollPanel.SetActive(false);
        BoothScrollPanel.SetActive(false);
        ExhibitionScrollPanel.SetActive(false);
        NoResult.SetActive(false);

        Utilities.DeleteAllChildGameObject(HistoryRoot);
        searchHistoryList = FileManager.LoadFile<List<string>>(SearchHistoryFileName);
        if (searchHistoryList == null)
        {
            searchHistoryList = new List<string>();
        }
        else
        {
            if (searchHistoryList.Count > 0)
            {
                Utilities.DeleteAllChildGameObject(HistoryRoot);
                int i = 0;
                searchHistoryList.Reverse();
                foreach (string s in searchHistoryList)
                {
                    GameObject newGrid = Instantiate(SearchHistoryPrefab);
                    newGrid.GetComponent<SearchMono>().Initialize(i, s);
                    Utilities.SetParentAndNormalize(newGrid.transform, HistoryRoot);
                    i++;
                }
                ResultText.text = "搜尋紀錄";
            }
        }
        DisplayTag();
    }

    private void changeTheMode()
    {
        if (currentMode == SearchMode.Product)
        {
            ProductButton.color = Selected;
            BoothButton.color = UnSelected;
            ExhibitionButton.color = UnSelected;
        }
        else if (currentMode == SearchMode.Booth)
        {
            ProductButton.color = UnSelected;
            BoothButton.color = Selected;
            ExhibitionButton.color = UnSelected;
        }
        else if (currentMode == SearchMode.Exhibition)
        {
            ProductButton.color = UnSelected;
            BoothButton.color = UnSelected;
            ExhibitionButton.color = Selected;
        }
        DisplayTag();
    }

    public void OnInputSearchClicked()
    {
        string tagName = Input.text;
        if (tagName.Length > 0)
        {
            OnSearch(Input.text);
            Input.text = "";
        }
        else
        {
            PopUp.Singleton.ShowError("請輸入有效文字", false);
        }
    }

    public void OnSearch(string tagName)
    {
        if (tagName.Length > 0)
        {
            if (!changedMode)
                currentMode = SearchMode.Product;
            PopUp.Singleton.ShowLoading();
            EESAPI.SearchProduct(new SearchProduct(tagName),
                (response) =>
                {
                    foreach (EESProduct eesProduct in response.data)
                    {
                        Product product = Product.Convert(eesProduct);
                        if (product != null)
                        {
                            searchedProduct.Add(product);
                        }
                    }
                    SearchBooth(tagName);
                },
                (error) =>
                {
                    if (error == CommonError.Rejected)
                        PopUp.Singleton.ShowError("伺服器拒絕存取\r\n即將關閉程式", false, () => { Application.Quit(); });
                    else if (error == CommonError.Timeout)
                    {
                        PopUp.Singleton.ShowError("連線逾時", true, () => { OnSearch(tagName); });
                    }
                    else if (error == CommonError.Unknown)
                    {
                        PopUp.Singleton.ShowError("未知錯誤", false);
                    }
                    //NoExhibition.SetActive(true);
                    PopUp.Singleton.CloseLoading();
                });
        }
        else
        {
            PopUp.Singleton.ShowError("搜尋欄位不可為空白", false);
        }
    }

    private void SearchBooth(string tagName)
    {
        EESAPI.SearchBooth(new SearchBooth(tagName),
                (response) =>
                {
                    foreach (EESBooth eesBooth in response.data)
                    {
                        Booth booth = Booth.Convert(eesBooth);
                        if (booth != null)
                        {
                            searchedBooth.Add(booth);
                        }
                    }
                    SearchExhibition(tagName);
                },
                (error) =>
                {
                    if (error == CommonError.Rejected)
                        PopUp.Singleton.ShowError("伺服器拒絕存取\r\n即將關閉程式", false, () => { Application.Quit(); });
                    else if (error == CommonError.Timeout)
                    {
                        PopUp.Singleton.ShowError("連線逾時", true, () => { SearchBooth(tagName); });
                    }
                    else if (error == CommonError.Unknown)
                    {
                        PopUp.Singleton.ShowError("未知錯誤", false);
                    }
                    //NoExhibition.SetActive(true);
                    PopUp.Singleton.CloseLoading();
                });
    }

    private void SearchExhibition(string tagName)
    {
        EESAPI.SearchExhibition(new SearchExhibition(tagName),
            (response) =>
            {
                foreach (EESExhibition eesExhibition in response.data)
                {
                    Exhibition exhibition = Exhibition.Convert(eesExhibition);
                    if (exhibition != null)
                    {
                        searchedExhibition.Add(exhibition);
                    }
                }
                DisplayTag();
                PopUp.Singleton.CloseLoading();
                AddHistory(tagName);
            },
            (error) =>
            {
                if (error == CommonError.Rejected)
                    PopUp.Singleton.ShowError("伺服器拒絕存取\r\n即將關閉程式", false, () => { Application.Quit(); });
                else if (error == CommonError.Timeout)
                {
                    PopUp.Singleton.ShowError("連線逾時", true, () => { SearchExhibition(tagName); });
                }
                else if (error == CommonError.Unknown)
                {
                    PopUp.Singleton.ShowError("未知錯誤", false);
                }
                //NoExhibition.SetActive(true);
                PopUp.Singleton.CloseLoading();
            });
    }

    public void DisplayTag()
    {
        int count = 0;
        HistoryScrollPanel.SetActive(false);
        ProductScrollPanel.SetActive(false);
        BoothScrollPanel.SetActive(false);
        ExhibitionScrollPanel.SetActive(false);

        if (currentMode == SearchMode.Search)
        {
            DisplayScrollPanel(HistoryScrollPanel);
            Utilities.DeleteAllChildGameObject(HistoryRoot);
            int i = 0;
            foreach (string p in searchHistoryList)
            {
                GameObject newGrid = Instantiate(SearchHistoryPrefab);
                newGrid.GetComponent<SearchMono>().Initialize(i, p);
                Utilities.SetParentAndNormalize(newGrid.transform, HistoryRoot);
                i++;
                count++;
            }
            ClearAll.SetActive(true);
            ClearResult.SetActive(false);
        }
        else
        {
            ClearAll.SetActive(false);
            ClearResult.SetActive(true);
        }

        if (currentMode == SearchMode.Product)
        {
            DisplayScrollPanel(ProductScrollPanel);
            Utilities.DeleteAllChildGameObject(ProductRoot);
            foreach (Product p in searchedProduct)
            {
                GameObject newGrid = Instantiate(SearchProductPrefab);
                newGrid.GetComponent<ProductMono>().Initialize(p.ProductName, p.Description, p.Price);
                Utilities.SetParentAndNormalize(newGrid.transform, ProductRoot);
                count++;
            }
        }
        else if (currentMode == SearchMode.Booth)
        {
            DisplayScrollPanel(BoothScrollPanel);
            Utilities.DeleteAllChildGameObject(BoothRoot);
            int i = 0;
            foreach (Booth b in searchedBooth)
            {
                GameObject newGrid = Instantiate(SearchBoothPrefab);
                newGrid.GetComponent<BoothMono>().Initialize(i, b, BoothDetails);
                Utilities.SetParentAndNormalize(newGrid.transform, BoothRoot);
                i++;
                count++;
            }
        }
        else if (currentMode == SearchMode.Exhibition)
        {
            DisplayScrollPanel(ExhibitionScrollPanel);
            Utilities.DeleteAllChildGameObject(ExhibitionRoot);
            int i = 0;
            foreach (Exhibition e in searchedExhibition)
            {
                GameObject newGrid = Instantiate(SearchExhibitionPrefab);
                newGrid.GetComponent<ExhibitionMono>().Initialize(i, e, ExhibitionDetails);
                Utilities.SetParentAndNormalize(newGrid.transform, ExhibitionRoot);
                i++;
                count++;
            }
        }

        if (count == 0)
            NoResult.SetActive(true);
        else
            NoResult.SetActive(false);
    }

    private void DisplayScrollPanel(GameObject target)
    {
        if (target == HistoryScrollPanel)
            HistoryScrollPanel.SetActive(true);
        else if (target == ProductScrollPanel)
            ProductScrollPanel.SetActive(true);
        else if (target == BoothScrollPanel)
            BoothScrollPanel.SetActive(true);
        else if (target == ExhibitionScrollPanel)
            ExhibitionScrollPanel.SetActive(true);
    }

    public void OnModeClicked(int mode)
    {
        changedMode = true;
        currentMode = (SearchMode)mode;
        changeTheMode();
    }

    public void AddHistory(string tagName)
    {
        if (!searchHistoryList.Contains(tagName))
            searchHistoryList.Add(tagName);
        FileManager.SaveFile(SearchHistoryFileName, searchHistoryList);
    }

    public void RemoveHistory()
    {
        searchHistoryList.Clear();
        Utilities.DeleteAllChildGameObject(HistoryRoot);
        FileManager.SaveFile(SearchHistoryFileName, searchHistoryList);
        PopUp.Singleton.ShowMessage("已清除搜尋紀錄");
    }

    public void OnInputFieldClicked()
    {
        Utilities.Transition(HistoryRoot.gameObject);
    }
}
