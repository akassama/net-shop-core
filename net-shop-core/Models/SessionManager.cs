using Microsoft.AspNetCore.Http;

public class SessionManager
{
    private readonly ISession _session;
    private const string ID_KEY = "_ID";
    private const string LOGIN_KEY = "_LoginName";
    private const string LOGIN_EMAIL = "_LoginEmail";
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
    public string LoginName
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

}