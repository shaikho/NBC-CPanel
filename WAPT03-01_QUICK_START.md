# WAPT03-01: Role-Based Authorization Filter — Quick Start Guide

## What You Get

✅ **AuthorizeRoleAttribute.cs** — A reusable, flexible role-based access control filter

### Key Capabilities:
- **Simple syntax:** `[AuthorizeRole("1,2,3")]`
- **Flexible:** Class-level or method-level application
- **Secure:** Returns 403 Forbidden for unauthorized access
- **Fast:** Runs on every request; early rejection saves resources

---

## How to Use It

### Example 1: Admin-Only Controller
```csharp
using AljazeeraCPanel.Filters;

[AuthorizeSession]
[AuthorizeRole("1")]  // Only role 1 (Super Admin)
public class AdminController : Controller
{
	public ActionResult Index() { /* everyone with role 1 */}
	public ActionResult ManageRoles() { /* everyone with role 1 */}
}
```

### Example 2: Mixed Roles with Overrides
```csharp
[AuthorizeSession]
[AuthorizeRole("1,2,3")]  // Default: roles 1, 2, 3 allowed
public class ReportController : Controller
{
	public ActionResult Dashboard() { /* roles 1,2,3 */ }

	[AuthorizeRole("1")]  // Override: Super Admin only
	public ActionResult SensitiveReport() { /* role 1 only */ }

	[AuthorizeRole("1,2")]  // Override: Super Admin or Admin
	public ActionResult ApproveRequests() { /* roles 1,2 */ }
}
```

### Example 3: No Role Restriction (Auth Only)
```csharp
[AuthorizeSession]
[AuthorizeRole]  // Empty = no role check; just check session
public class ProfileController : Controller
{
	public ActionResult MyProfile() { /* any logged-in user */ }
}
```

---

## Role IDs Reference

| Role ID | Role Name | Typical Permissions |
|---------|-----------|---|
| **1** | Super Admin / System Admin | Full system access, can manage all |
| **2** | Admin / Branch Manager | Branch-level management, user/role management |
| **3** | Officer / Staff | View/process requests, standard operations |
| **4+** | Custom Roles | As defined in database |

**To find exact role IDs:** Query `SELECT * FROM tbl_rolemaster;` in Oracle database

---

## How It Works

```
User makes request to protected action
		↓
[AuthorizeSessionAttribute] checks
	- Is user logged in?
	- Does session have required vars?
		↓ NO → Redirect to Login
		↓ YES
[AuthorizeRoleAttribute] checks
	- Does user's role match allowed roles?
		↓ NO → Return 403 Forbidden
		↓ YES → Execute action
```

---

## Error Messages

| User Sees | Cause | Solution |
|-----------|-------|----------|
| **Redirects to Login** | Not authenticated | Log in |
| **403 Forbidden Page** | Logged in but wrong role | Use account with right role |
| **White page** | Redirect configured (optional) | Check configuration |

---

## Example Application Walkthrough

### Scenario 1: Super Admin (Role 1) logs in
```
User: admin@bank.com (Role 1)

Request: GET /Admin/ManageRoles
[AuthorizeSession]: ✅ Logged in
[AuthorizeRole("1")]: ✅ Role 1 is in allowed list
Result: ✅ Access granted → Page shows
```

### Scenario 2: Staff (Role 3) tries admin action
```
User: officer@bank.com (Role 3)

Request: GET /Admin/ManageRoles
[AuthorizeSession]: ✅ Logged in
[AuthorizeRole("1")]: ❌ Role 3 is NOT in allowed list
Result: ❌ HTTP 403 Forbidden → Error page
```

### Scenario 3: No login attempts protected action
```
User: Not logged in

Request: GET /Admin/ManageRoles
[AuthorizeSession]: ❌ No valid session
Result: ❌ Redirect to /Login/Login
```

---

## Implementation Checklist

- [ ] **Review** `WAPT03-01_IMPLEMENTATION.md` for detailed docs
- [ ] **Understand** your application's role structure
- [ ] **Identify** high-risk actions (deletions, approvals, admin functions)
- [ ] **Start applying** `[AuthorizeRole]` to:
  - [ ] Admin controllers (CPanelProfileManagement, User, Service, etc.)
  - [ ] Risk controllers (DeleteCustomer, DeActiveAccount, ResetCustomer)
  - [ ] Approval controllers (ActiveAccount, CustomerAuthorization)
- [ ] **Test** each role scenario
- [ ] **Document** role requirements in code comments
- [ ] **Proceed** to WAPT03-02 for full rollout

---

## Common Mistakes to Avoid

❌ **Don't:** `[AuthorizeRole("1,2")]` without `[AuthorizeSession]`
✅ **Do:** Always pair with `[AuthorizeSession]`

❌ **Don't:** Use different role numbers inconsistently
✅ **Do:** Document standard role IDs in your team wiki

❌ **Don't:** Leave `[AuthorizeRole]` on sensitive actions with no role specified
✅ **Do:** Always specify required roles explicitly

❌ **Don't:** Mix with old manual session checks
✅ **Do:** Replace all manual checks with the filter attribute

---

## Performance Implications

- **No database calls** — Reads only from session
- **O(1) lookup** — Role string comparison
- **~1-2ms overhead** per request (negligible)
- **No caching needed** — Session is already cached

---

## Next Steps

**WAPT03-02:** Apply `[AuthorizeRole]` to all protected controller actions

**WAPT03-03:** Add server-side role validation inside action methods (defense in depth)

---

## Support / Questions

See `WAPT03-01_IMPLEMENTATION.md` for:
- Detailed architecture
- Testing strategies
- Best practices
- Future enhancements
- Compliance info

---

**Status:** ✅ **READY FOR IMPLEMENTATION**  
**Next Session:** WAPT03-02 (Apply to all controllers)
