using System;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using EES.ClientAPIs;
using EES.ClientAPIs.ClientModules;
using UnityEngine;
using EES.Utilities;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace EES
{
    namespace ClientAPIs
    {
        #region ClientModules
        /// <summary>
        /// Client API modules for sending receiving request to server
        /// </summary>
        namespace ClientModules
        {
            #region Class Modules
            /// <summary>
            /// User data.
            /// </summary>
            public class EESUserData
            {
                public int accountId;
                public string displayName = null;
                public DateTime birthday;
                public bool isMale = true;
                public EESUserData(int accountId, string displayName, DateTime birthday, string isMale)
                {
                    this.accountId = accountId;
                    this.displayName = displayName;
                    try
                    {
                        this.birthday = birthday;
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                        this.birthday = DateTime.Now;
                    }
                    if (isMale == "1")
                        this.isMale = true;
                    else
                        this.isMale = false;
                }
            }

            public class EESExhibition
            {
                public int exhibitionId;
                public int organizerId;
                public string displayName;
                public string location;
                public DateTime startDate;
                public DateTime endDate;
                public string description;
                public int popularity;
                public EESExhibition(int exhibitionId, int organizerId, string displayName, string location,
                    DateTime startDate, DateTime endDate, string description, int popularity)
                {
                    this.exhibitionId = exhibitionId;
                    this.organizerId = organizerId;
                    this.displayName = displayName;
                    this.location = location;
                    this.startDate = startDate;
                    this.endDate = endDate;
                    this.description = description;
                    this.popularity = popularity;
                }
            }

            public class EESBooth
            {
                public int boothId;
                public string boothPosition;
                public int exhibitionId;
                public int holderId;
                public string holderName;
                public string displayName;
                public List<string> boothTag;
                public int popularity;
                public EESBooth(int boothId, string boothPosition, int exhibitionId, int holderId, string holderName,
                    string displayName, List<string> boothTag, int popularity)
                {
                    this.boothId = boothId;
                    this.boothPosition = boothPosition;
                    this.exhibitionId = exhibitionId;
                    this.holderId = holderId;
                    this.holderName = holderName;
                    this.displayName = displayName;
                    if (boothTag == null)
                        this.boothTag = new List<string>();
                    else
                        this.boothTag = boothTag;
                    this.popularity = popularity;
                }
            }

            public class EESProduct
            {
                public int productId;
                public int holderId;
                public string holderName;
                public string productName;
                public string description;
                public int price;
                public EESProduct(int productId, int holderId, string holderName, string productName, string description, int price)
                {
                    this.productId = productId;
                    this.holderId = holderId;
                    this.holderName = holderName;
                    this.productName = productName;
                    this.description = description;
                    this.price = price;
                }
            }
            #endregion

            #region Error Modules
            /// <summary>
            /// Login error callback result 
            /// </summary>
            public enum LoginError
            {
                /// <summary>
                /// Login success
                /// </summary>
                Success,
                /// <summary>
                /// Email is not exist
                /// </summary>
                EmailNotFound,
                /// <summary>
                /// Invalid password
                /// </summary>
                WrongPassword,
                /// <summary>
                /// Didn't get response from server after a period of time
                /// </summary>
                Timeout,
                /// <summary>
                /// Undefined error
                /// </summary>
                Unknown
            }

            /// <summary>
            /// Register error callback result
            /// </summary>
            public enum RegisterError
            {
                /// <summary>
                /// Email is exist
                /// </summary>
                EmailIsExist,
                /// <summary>
                /// Invalid input
                /// </summary>
                InvalidInput,
                /// <summary>
                /// The token is invalid or expired, do not login in multi devices
                /// </summary>
                InvalidOrExpiredToken,
                /// <summary>
                /// DataInsertFailed
                /// </summary>
                DataInsertFailed,
                /// <summary>
                /// Didn't get response from server after a period of time
                /// </summary>
                Timeout,
                /// <summary>
                /// Undefined error
                /// </summary>
                Unknown
            }

            /// <summary>
            /// Common error callback result
            /// </summary>
            public enum CommonError
            {
                /// <summary>
                /// The token is invalid or expired, do not login in multi devices
                /// </summary>
                InvalidOrExpiredToken,
                /// <summary>
                /// No requested objects found in user data
                /// </summary>
                NotFound,
                /// <summary>
                /// Didn't get response from server after a period of time
                /// </summary>
                Timeout,
                /// <summary>
                /// Data insert failed
                /// </summary>
                DataInsertFailed,
                /// <summary>
                /// Server reject the request
                /// </summary>
                Rejected,
                /// <summary>
                /// Undefined error
                /// </summary>
                Unknown
            }

            /// <summary>
            /// FB error callback result
            /// </summary>
            public enum FBError
            {
                /// <summary>
                /// Invalid password
                /// </summary>
                WrongPassword,
                /// <summary>
                /// Data insert failed
                /// </summary>
                DataInsertFailed,
                /// <summary>
                /// Didn't get response from server after a period of time
                /// </summary>
                Timeout,
                /// <summary>
                /// Undefined error
                /// </summary>
                Unknown
            }
            #endregion

            #region Core Modules

            /// <summary>
            /// POST method encoding
            /// </summary>
            public enum ContentType
            {
                x_www_form_urlencoded,
                json
            }

            /// <summary>
            /// sending request and receiving response with server
            /// </summary>
            public class Communication
            {
                public string getServerResponse { get { return serverResponse; } }
                private string serverResponse;
                public bool IsSwitchOn { get { return isSwitchOn; } }
                private bool isSwitchOn = true;
                private string strPost;
                private string EESService;
                private ContentType contentType;
                public Communication(string _strPost, string _eesService, ContentType _contentType)
                {
                    strPost = _strPost;
                    EESService = _eesService;
                    contentType = _contentType;
                }

                #region Talk function
                /// <summary>
                /// send request to server and store response in <see cref="serverResponse"/>: 
                /// </summary>
                public void Talk()
                {
                    isSwitchOn = true;
                    Debug.Log("Starting Talk thread. Service: " + EESService + ", strPost: " + strPost);

                    StreamWriter myWriter = null;
                    StreamReader myReader = null;
                    HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(EESAPI.ServerIP + EESService);
                    objRequest.Method = "POST";
                    objRequest.ContentLength = Encoding.UTF8.GetBytes(strPost).Length;

                    if (contentType == ContentType.x_www_form_urlencoded)
                        objRequest.ContentType = "application/x-www-form-urlencoded";
                    else if (contentType == ContentType.json)
                        objRequest.ContentType = "application/json";

                    try
                    {
                        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
                        myWriter = new StreamWriter(objRequest.GetRequestStream());
                        myWriter.Write(strPost);
                        myWriter.Close();
                        HttpWebResponse response = (HttpWebResponse)objRequest.GetResponse();
                        myReader = new StreamReader(response.GetResponseStream());
                        serverResponse = myReader.ReadToEnd();
                        Debug.Log("Response in Talk thread: " + serverResponse + " \nService: " + EESService + ", strPost: " + strPost);
                        myReader.Close();
                        response.Close();
                        isSwitchOn = false;
                        return;
                    }
                    catch (Exception e)
                    {
                        isSwitchOn = false;
                        serverResponse = null;
                        Debug.Log("Exception in Talk thread trigged: " + e);
                    }
                }

                public bool MyRemoteCertificateValidationCallback(System.Object sender,
    X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                {
                    bool isOk = true;
                    // If there are errors in the certificate chain,
                    // look at each error to determine the cause.
                    if (sslPolicyErrors != SslPolicyErrors.None)
                    {
                        for (int i = 0; i < chain.ChainStatus.Length; i++)
                        {
                            if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown)
                            {
                                continue;
                            }
                            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                            chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                            chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                            bool chainIsValid = chain.Build((X509Certificate2)certificate);
                            if (!chainIsValid)
                            {
                                isOk = false;
                                break;
                            }
                        }
                    }
                    return isOk;
                }
                #endregion

                /// <summary>
                /// static function for sending and receiving data with server.
                /// </summary>
                /// <param name="_strPost">The string to post.</param>
                /// <param name="_EESService">The EES service called from client.</param>
                /// <param name="_contentType">POST method encoding.</param>
                /// <param name="_callBack">Result callBack. The first parameter indicating whether time out or not. The second one contains string result return from server</param>
                public static void CommunicateWithService(string _strPost, string _EESService, ContentType _contentType, Action<bool, string> _callBack)
                {
                    StaticMono.Instance.StartCoroutine(StartConnectCor(_strPost, _EESService, _contentType, _callBack));
                }

                /// <summary>
                /// Coroutine for checking thread status
                /// </summary>
                public static IEnumerator StartConnectCor(string _strPost, string _EESService, ContentType _contentType, Action<bool, string> _callBack)
                {
                    Communication _communicationThread = new Communication(_strPost, _EESService, _contentType);
                    Thread thread = new Thread(_communicationThread.Talk);
                    thread.Start();
                    for (int _timer = 0; ; _timer++)
                    {
                        if (_timer > EESAPI.Timeout)
                        {
                            thread.Abort();
                            _callBack(true, null);
                            yield break;
                        }
                        if (!_communicationThread.IsSwitchOn)
                        {
                            thread.Abort();
                            _callBack(false, _communicationThread.getServerResponse);
                            yield break;
                        }
                        yield return new WaitForSeconds(1);
                    }
                }
            }


            #region Request Modules
            public class GeneralRequest
            {
                public int accountId;
                public GeneralRequest()
                {
                    accountId = AccountManager.GetAccount.AccountId;
                }
            }

            /// <summary>
            /// required login parameters
            /// </summary>
            public class LoginRequest
            {
                public string email = null;
                public string password = null;
                public LoginRequest(string email, string password)
                {
                    this.email = email;
                    this.password = password;
                }
            }

            public class SignRequest
            {
                public string email = null;
                public string password = null;
                public string displayName = null;
                public DateTime birthday;
                public bool isMale;
                public SignRequest(string email, string password, string displayName, DateTime birthday, bool isMale)
                {
                    this.email = email;
                    this.password = password;
                    this.displayName = displayName;
                    this.birthday = birthday;
                    this.isMale = isMale;
                }
            }

            public class GetBoothRequest
            {
                public int exhibitionId;
                public GetBoothRequest(int exhibitionId)
                {
                    this.exhibitionId = exhibitionId;
                }
            }

            public class GetProductRequest
            {
                public int boothId;
                public GetProductRequest(int boothId)
                {
                    this.boothId = boothId;
                }
            }

            public class SearchProduct
            {
                public string productTagName;
                public SearchProduct(string productTagName)
                {
                    this.productTagName = productTagName;
                }
            }

            public class SearchBooth
            {
                public string boothTagName;
                public SearchBooth(string boothTagName)
                {
                    this.boothTagName = boothTagName;
                }
            }

            public class SearchExhibition
            {
                public string keyword;
                public SearchExhibition(string keyword)
                {
                    this.keyword = keyword;
                }
            }

            public class SubscribeRequest
            {
                public int subscribeAccountId;
                public int subscribedAccountId;
                public SubscribeRequest(int subscribedAccountId)
                {
                    this.subscribeAccountId = AccountManager.GetAccount.AccountId;
                    this.subscribedAccountId = subscribedAccountId;
                }
            }  
            #endregion

            #region Response Modules
            /// <summary>
            /// Response return by server.
            /// </summary>
            public class Response
            {
                /// <summary>
                /// What kind of function called on server
                /// </summary>
                public string type;
                /// <summary>
                /// The status of server response. See EES client API for more details
                /// </summary>
                public string status;
                public int error;
            }

            /// <summary>
            /// Login response return by server.
            /// </summary>
            public class LoginResponse : Response
            {
                public string token;
                public EESUserData data;
            }

            public class SignUpResponse : Response
            {
                public string token;
                public EESUserData data;
            }

            public class ExhibitionResponse : Response
            {
                public List<EESExhibition> data;
            }

            public class BoothResponse : Response
            {
                public List<EESBooth> data;
            }

            public class ProductResponse : Response
            {
                public List<EESProduct> data;
            }

            public class SubscribeList
            {
                public List<Subscribe> subscribers;
                public List<Subscribe> subscribed;
            }

            public class GetSubscribeResponse : Response
            {
                public SubscribeList data;
            }

            public class SubscribeResponse : Response
            {
                public List<EESUserData> data;
            }

            #endregion
            #endregion
        }
        #endregion

        /// <summary>
        /// APIs for sending request to server
        /// </summary>
        public class EESAPI
        {
            public const string ServerIP = "https://yochien.tk/EES";
            public const string OK = "ok";

            /// <summary>
            /// Current API version
            /// </summary>
            public const string version = "1.0";

            /// <summary>
            /// Time out limit
            /// </summary>
            public static float Timeout = 10.0f;

            /// <summary>
            /// User's token for sending request to server
            /// </summary>
            public static string Token { get { return token; } }
            private static string token = null;

            public static bool IsLoggedIn { get { return isLoggedIn; } }
            private static bool isLoggedIn = false;

            /// </example>
            /// <param name="_request">required login parameters</param>
            /// <param name="SuccessCallback">Callback with user's token. Invoked when login success. </param>
            /// <param name="ErrorCallback">Callback with error message. Invoked when login failed.</param>
            static public void Login(LoginRequest _request, Action<EESUserData> SuccessCallback, Action<LoginError> ErrorCallback)
            {
                //string strPost = "email=" + _request.email + "&password=" + _request.password;
                //Communication.CommunicateWithService(strPost, "/login", ContentType.x_www_form_urlencoded,
                string strPost = JsonConvert.SerializeObject(_request);
                Communication.CommunicateWithService(strPost, "/EES_login.php", ContentType.json,
                    //call back
                    (_isTimeOut, _result) =>
                    {
                        if (_isTimeOut)
                        {
                            ErrorCallback(LoginError.Timeout);
                            return;
                        }

                        LoginResponse response;

                        try
                        {
                            response = JsonConvert.DeserializeObject<LoginResponse>(_result);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("Login error occurred: " + e + ", Json data: " + _result);
                            ErrorCallback(LoginError.Unknown);
                            return;
                        }

                        if (response.status == OK)
                        {
                            Debug.Log("Login success.");
                            token = response.token;
                            isLoggedIn = true;
                            SuccessCallback(response.data);
                            return;
                        }
                        else
                        {
                            int error_code = response.error;
                            if (error_code == 101)
                            {
                                Debug.Log("Email not found.");
                                ErrorCallback(LoginError.EmailNotFound);
                            }
                            else if (error_code == 102)
                            {
                                Debug.Log("Wrong password");
                                ErrorCallback(LoginError.WrongPassword);
                            }
                            else
                            {
                                Debug.Log("Unknown error: " + response);
                                ErrorCallback(LoginError.Unknown);
                            }
                            Debug.Log("Error when login: " + response.error);
                        }
                    }
                );
            }

            /// <summary>
            /// Register a new user
            /// </summary>
            /// <param name="_request">required register parameters</param>
            /// <param name="status">function which you want to call on server. "/signup" is for first time register. "/Update" is for updating user profile.</param>
            /// <param name="SuccessCallback">Callback with result. Invoked when register success.</param>
            /// <param name="ErrorCallback">Callback with error message. Invoked when register failed.</param>
            static public void SignUp(SignRequest _request, Action<int> SuccessCallback, Action<RegisterError> ErrorCallback)
            {
                string strPost = JsonConvert.SerializeObject(_request);
                Communication.CommunicateWithService(strPost, "/EES_signup.php", ContentType.json,
                    //call back
                    (_isTimeOut, _result) =>
                    {
                        if (_isTimeOut)
                        {
                            ErrorCallback(RegisterError.Timeout);
                            return;
                        }

                        SignUpResponse response = null;

                        try
                        {
                            response = JsonConvert.DeserializeObject<SignUpResponse>(_result);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("Register error occurred" + e + ", Json data: " + _result);
                            ErrorCallback(RegisterError.Unknown);
                            return;
                        }

                        if (response.status == OK)
                        {
                            Debug.Log("Register and Login success.");
                            if (response.type == "signup")
                            {
                                token = response.token;
                                isLoggedIn = true;
                                SuccessCallback(response.data.accountId);
                            }
                            return;
                        }
                        else
                        {
                            int errorCode = response.error;
                            if (errorCode == 201)
                            {
                                Debug.Log("email is already existed.");
                                ErrorCallback(RegisterError.EmailIsExist);
                            }
                            else if (errorCode == 202)
                            {
                                Debug.Log("invalid input.");
                                ErrorCallback(RegisterError.InvalidInput);
                            }
                            else if (errorCode == 203)
                            {
                                Debug.Log("save failed.");
                                ErrorCallback(RegisterError.DataInsertFailed);
                            }
                            else
                            {
                                Debug.Log("unknown error: " + response);
                                ErrorCallback(RegisterError.Unknown);
                            }
                            Debug.Log(response.error);
                        }
                    }
                );
            }

            /// <summary>
            /// Register a new user
            /// </summary>
            /// <param name="_request">required register parameters</param>
            /// <param name="status">function which you want to call on server. "/signup" is for first time register. "/Update" is for updating user profile.</param>
            /// <param name="SuccessCallback">Callback with result. Invoked when register success.</param>
            /// <param name="ErrorCallback">Callback with error message. Invoked when register failed.</param>
            static public void GetExhibtion(GeneralRequest _request, Action<ExhibitionResponse> SuccessCallback, Action<CommonError> ErrorCallback)
            {
                string strPost = JsonConvert.SerializeObject(_request);
                Communication.CommunicateWithService(strPost, "/EES_getExhibtion.php", ContentType.json,
                    //call back
                    (_isTimeOut, _result) =>
                    {
                        if (_isTimeOut)
                        {
                            ErrorCallback(CommonError.Timeout);
                            return;
                        }

                        ExhibitionResponse response = null;

                        try
                        {
                            response = JsonConvert.DeserializeObject<ExhibitionResponse>(_result);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("Exhibition error occurred" + e + ", Json data: " + _result);
                            ErrorCallback(CommonError.Unknown);
                            return;
                        }

                        if (response.status == OK)
                        {
                            Debug.Log("getExhibtion success.");
                            if (response.type == "getExhibtion")
                            {
                                SuccessCallback(response);
                            }
                            return;
                        }
                        else
                        {
                            int errorCode = response.error;
                            if (errorCode == 301)
                            {
                                Debug.Log("server rejected.");
                                ErrorCallback(CommonError.Rejected);
                            }
                            else
                            {
                                Debug.Log("unknown error: " + response);
                                ErrorCallback(CommonError.Unknown);
                            }
                            Debug.Log(response.error);
                        }
                    }
                );
            }

            static public void GetBooth(GetBoothRequest _request, Action<BoothResponse> SuccessCallback, Action<CommonError> ErrorCallback)
            {
                string strPost = JsonConvert.SerializeObject(_request);
                Communication.CommunicateWithService(strPost, "/EES_getBooth.php", ContentType.json,
                    //call back
                    (_isTimeOut, _result) =>
                    {
                        if (_isTimeOut)
                        {
                            ErrorCallback(CommonError.Timeout);
                            return;
                        }

                        BoothResponse response = null;

                        try
                        {
                            response = JsonConvert.DeserializeObject<BoothResponse>(_result);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("Booth error occurred" + e + ", Json data: " + _result);
                            ErrorCallback(CommonError.Unknown);
                            return;
                        }

                        if (response.status == OK)
                        {
                            Debug.Log("getBooth success.");
                            if (response.type == "getBooth")
                            {
                                SuccessCallback(response);
                            }
                            return;
                        }
                        else
                        {
                            int errorCode = response.error;
                            if (errorCode == 401)
                            {
                                Debug.Log("server rejected.");
                                ErrorCallback(CommonError.Rejected);
                            }
                            else
                            {
                                Debug.Log("unknown error: " + response);
                                ErrorCallback(CommonError.Unknown);
                            }
                            Debug.Log(response.error);
                        }
                    }
                );
            }

            static public void GetProduct(GetProductRequest _request, Action<ProductResponse> SuccessCallback, Action<CommonError> ErrorCallback)
            {
                string strPost = JsonConvert.SerializeObject(_request);
                Communication.CommunicateWithService(strPost, "/EES_getProduct.php", ContentType.json,
                    //call back
                    (_isTimeOut, _result) =>
                    {
                        if (_isTimeOut)
                        {
                            ErrorCallback(CommonError.Timeout);
                            return;
                        }

                        ProductResponse response = null;

                        try
                        {
                            response = JsonConvert.DeserializeObject<ProductResponse>(_result);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("Products error occurred" + e + ", Json data: " + _result);
                            ErrorCallback(CommonError.Unknown);
                            return;
                        }

                        if (response.status == OK)
                        {
                            Debug.Log("getProduct success.");
                            if (response.type == "getProduct")
                            {
                                SuccessCallback(response);
                            }
                            return;
                        }
                        else
                        {
                            int errorCode = response.error;
                            if (errorCode == 501)
                            {
                                Debug.Log("server rejected.");
                                ErrorCallback(CommonError.Rejected);
                            }
                            else
                            {
                                Debug.Log("unknown error: " + response);
                                ErrorCallback(CommonError.Unknown);
                            }
                            Debug.Log(response.error);
                        }
                    }
                );
            }

            static public void SearchProduct(SearchProduct _request, Action<ProductResponse> SuccessCallback, Action<CommonError> ErrorCallback)
            {
                string strPost = JsonConvert.SerializeObject(_request);
                Communication.CommunicateWithService(strPost, "/EES_searchInTheProduct.php", ContentType.json,
                    //call back
                    (_isTimeOut, _result) =>
                    {
                        if (_isTimeOut)
                        {
                            ErrorCallback(CommonError.Timeout);
                            return;
                        }

                        ProductResponse response = null;

                        try
                        {
                            response = JsonConvert.DeserializeObject<ProductResponse>(_result);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("SearchProducts error occurred" + e + ", Json data: " + _result);
                            ErrorCallback(CommonError.Unknown);
                            return;
                        }

                        if (response.status == OK)
                        {
                            Debug.Log("SearchProduct success.");
                            if (response.type == "searchInTheProduct")
                            {
                                SuccessCallback(response);
                            }
                            return;
                        }
                        else
                        {
                            int errorCode = response.error;
                            if (errorCode == 601)
                            {
                                Debug.Log("server rejected.");
                                ErrorCallback(CommonError.Rejected);
                            }
                            else
                            {
                                Debug.Log("unknown error: " + response);
                                ErrorCallback(CommonError.Unknown);
                            }
                            Debug.Log(response.error);
                        }
                    }
                );
            }

            static public void SearchBooth(SearchBooth _request, Action<BoothResponse> SuccessCallback, Action<CommonError> ErrorCallback)
            {
                string strPost = JsonConvert.SerializeObject(_request);
                Communication.CommunicateWithService(strPost, "/EES_searchInTheBooth.php", ContentType.json,
                    //call back
                    (_isTimeOut, _result) =>
                    {
                        if (_isTimeOut)
                        {
                            ErrorCallback(CommonError.Timeout);
                            return;
                        }

                        BoothResponse response = null;

                        try
                        {
                            response = JsonConvert.DeserializeObject<BoothResponse>(_result);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("SearchBooths error occurred" + e + ", Json data: " + _result);
                            ErrorCallback(CommonError.Unknown);
                            return;
                        }

                        if (response.status == OK)
                        {
                            Debug.Log("SearchBooth success.");
                            if (response.type == "searchInTheBooth")
                            {
                                SuccessCallback(response);
                            }
                            return;
                        }
                        else
                        {
                            int errorCode = response.error;
                            if (errorCode == 701)
                            {
                                Debug.Log("server rejected.");
                                ErrorCallback(CommonError.Rejected);
                            }
                            else
                            {
                                Debug.Log("unknown error: " + response);
                                ErrorCallback(CommonError.Unknown);
                            }
                            Debug.Log(response.error);
                        }
                    }
                );
            }

            static public void SearchExhibition(SearchExhibition _request, Action<ExhibitionResponse> SuccessCallback, Action<CommonError> ErrorCallback)
            {
                string strPost = JsonConvert.SerializeObject(_request);
                Communication.CommunicateWithService(strPost, "/EES_searchInTheExhibition.php", ContentType.json,
                    //call back
                    (_isTimeOut, _result) =>
                    {
                        if (_isTimeOut)
                        {
                            ErrorCallback(CommonError.Timeout);
                            return;
                        }

                        ExhibitionResponse response = null;

                        try
                        {
                            response = JsonConvert.DeserializeObject<ExhibitionResponse>(_result);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("SearchExhibitions error occurred" + e + ", Json data: " + _result);
                            ErrorCallback(CommonError.Unknown);
                            return;
                        }

                        if (response.status == OK)
                        {
                            Debug.Log("SearchExhibition success.");
                            if (response.type == "searchInTheExhibition")
                            {
                                SuccessCallback(response);
                            }
                            return;
                        }
                        else
                        {
                            int errorCode = response.error;
                            if (errorCode == 801)
                            {
                                Debug.Log("server rejected.");
                                ErrorCallback(CommonError.Rejected);
                            }
                            else
                            {
                                Debug.Log("unknown error: " + response);
                                ErrorCallback(CommonError.Unknown);
                            }
                            Debug.Log(response.error);
                        }
                    }
                );
            }

            static public void AddSubscribe(SubscribeRequest _request, Action<SubscribeResponse> SuccessCallback, Action<CommonError> ErrorCallback)
            {
                string strPost = JsonConvert.SerializeObject(_request);
                Communication.CommunicateWithService(strPost, "/EES_subscribe.php", ContentType.json,
                    //call back
                    (_isTimeOut, _result) =>
                    {
                        if (_isTimeOut)
                        {
                            ErrorCallback(CommonError.Timeout);
                            return;
                        }

                        SubscribeResponse response = null;

                        try
                        {
                            response = JsonConvert.DeserializeObject<SubscribeResponse>(_result);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("subscribe error occurred" + e + ", Json data: " + _result);
                            ErrorCallback(CommonError.Unknown);
                            return;
                        }

                        if (response.status == OK)
                        {
                            Debug.Log("subscribe success.");
                            if (response.type == "subscribe")
                            {
                                SuccessCallback(response);
                            }
                            return;
                        }
                        else
                        {
                            int errorCode = response.error;
                            if (errorCode == 1001)
                            {
                                Debug.Log("server rejected.");
                                ErrorCallback(CommonError.Rejected);
                            }
                            else if(errorCode == 1002)
                            {
                                Debug.Log("Data insert failed.");
                                ErrorCallback(CommonError.DataInsertFailed);
                            }
                            else
                            {
                                Debug.Log("unknown error: " + response);
                                ErrorCallback(CommonError.Unknown);
                            }
                            Debug.Log(response.error);
                        }
                    }
                );
            }

            static public void UnSubscribe(SubscribeRequest _request, Action<SubscribeResponse> SuccessCallback, Action<CommonError> ErrorCallback)
            {
                string strPost = JsonConvert.SerializeObject(_request);
                Communication.CommunicateWithService(strPost, "/EES_unsubscribe.php", ContentType.json,
                    //call back
                    (_isTimeOut, _result) =>
                    {
                        if (_isTimeOut)
                        {
                            ErrorCallback(CommonError.Timeout);
                            return;
                        }

                        SubscribeResponse response = null;

                        try
                        {
                            response = JsonConvert.DeserializeObject<SubscribeResponse>(_result);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("unsubscribe error occurred" + e + ", Json data: " + _result);
                            ErrorCallback(CommonError.Unknown);
                            return;
                        }

                        if (response.status == OK)
                        {
                            Debug.Log("unsubscribe success.");
                            if (response.type == "unsubscribe")
                            {
                                SuccessCallback(response);
                            }
                            return;
                        }
                        else
                        {
                            int errorCode = response.error;
                            if (errorCode == 1101)
                            {
                                Debug.Log("server rejected.");
                                ErrorCallback(CommonError.Rejected);
                            }
                            else
                            {
                                Debug.Log("unknown error: " + response);
                                ErrorCallback(CommonError.Unknown);
                            }
                            Debug.Log(response.error);
                        }
                    }
                );
            }

            static public void GetSubscribe(GeneralRequest _request, Action<GetSubscribeResponse> SuccessCallback, Action<CommonError> ErrorCallback)
            {
                string strPost = JsonConvert.SerializeObject(_request);
                Communication.CommunicateWithService(strPost, "/EES_getSubscribe.php", ContentType.json,
                    //call back
                    (_isTimeOut, _result) =>
                    {
                        if (_isTimeOut)
                        {
                            ErrorCallback(CommonError.Timeout);
                            return;
                        }

                        GetSubscribeResponse response = null;

                        try
                        {
                            response = JsonConvert.DeserializeObject<GetSubscribeResponse>(_result);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("SearchExhibitions error occurred" + e + ", Json data: " + _result);
                            ErrorCallback(CommonError.Unknown);
                            return;
                        }

                        if (response.status == OK)
                        {
                            Debug.Log("getSubscribe success.");
                            if (response.type == "getSubscribe")
                            {
                                SuccessCallback(response);
                            }
                            return;
                        }
                        else
                        {
                            int errorCode = response.error;
                            if (errorCode == 1201)
                            {
                                Debug.Log("server rejected.");
                                ErrorCallback(CommonError.Rejected);
                            }
                            else
                            {
                                Debug.Log("unknown error: " + response);
                                ErrorCallback(CommonError.Unknown);
                            }
                            Debug.Log(response.error);
                        }
                    }
                );
            }

            /// <summary>
            /// Invoked when logout
            /// </summary>
            static public void Logout()
            {
                isLoggedIn = false;
                token = null;
            }
        }
    }


}