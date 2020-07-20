using Microsoft.AspNetCore.Http;

public class SessionManager
{
    private readonly ISession _session;
    private const string ID_KEY = "_ID";
    private const string LOGIN_KEY = "_AccountId";
    private const string LOGIN_EMAIL = "_LoginEmail";
    private const string LOGIN_USER_NAME = "_LoginUserName";
    private const string LOGIN_FIRST_NAME = "_LoginFirstName";
    private const string LOGIN_LAST_NAME = "_LoginLastName";
    private const string LOGIN_DIR_NAME = "_LoginDirectoryName";
    private const string LOGIN_PROFILE_PICTURE = "_LoginProfilePicture";
    public SessionManager(IHttpContextAccessor httpContextAccessor)
    {
        _session = httpContextAccessor.HttpContext.Session;
    }

    public int ID
    {
        get
        {
            var v = _session.GetInt32(ID_KEY);
            if (v.HasValue)
                return v.Value;
            else
                return 0;
        }
        set
        {
            _session.SetInt32(ID_KEY, value);
        }
    }
    public string LoginAccountId
    {
        get
        {
            return _session.GetString(LOGIN_KEY);
        }
        set
        {
            _session.SetString(LOGIN_KEY, value);
        }
    }

    public string LoginEmail
    {
        get
        {
            return _session.GetString(LOGIN_EMAIL);
        }
        set
        {
            _session.SetString(LOGIN_EMAIL, value);
        }
    }

    public string LoginUsername
    {
        get
        {
            return _session.GetString(LOGIN_USER_NAME);
        }
        set
        {
            _session.SetString(LOGIN_USER_NAME, value);
        }
    }

    public string LoginFirstName
    {
        get
        {
            return _session.GetString(LOGIN_FIRST_NAME);
        }
        set
        {
            _session.SetString(LOGIN_FIRST_NAME, value);
        }
    }

    public string LoginLastName
    {
        get
        {
            return _session.GetString(LOGIN_LAST_NAME);
        }
        set
        {
            _session.SetString(LOGIN_LAST_NAME, value);
        }
    }

    public string LoginDirectoryName
    {
        get
        {
            return _session.GetString(LOGIN_DIR_NAME);
        }
        set
        {
            _session.SetString(LOGIN_DIR_NAME, value);
        }
    }

    public string LoginProfilePicture
    {
        get
        {
            return _session.GetString(LOGIN_PROFILE_PICTURE);
        }
        set
        {
            _session.SetString(LOGIN_PROFILE_PICTURE, value);
        }
    }
    public bool IsLoggedIn
    {
        get
        {
            if (ID > 0)
                return true;
            else
                return false;
        }
    }

    //Clears user session data on logout
    public void ClearSessions()
    {
        _session.Remove("_ID");
        _session.Remove("_AccountId");
        _session.Remove("_LoginEmail");
        _session.Remove("_LoginUserName");
        _session.Remove("_LoginFirstName");
        _session.Remove("_LoginLastName");
        _session.Remove("_LoginDirectoryName");
        _session.Remove("_LoginProfilePicture");
    }

}