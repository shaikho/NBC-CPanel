# WAPT03-01: Controller Role Mapping Guide

## Purpose
This guide maps each controller to recommended role restrictions based on the sensitivity of operations and typical banking/admin organizational structures.

**Recommended Role Structure:**
- **Role 1** = Super Admin (System-wide access, can modify everything)
- **Role 2** = Admin/Branch Manager (Branch-level management, user/role creation)
- **Role 3** = Officer/Staff (Standard operations, process requests)
- **Role 4+** = Custom roles as needed

---

## Controller Role Mapping

### 🔴 **CRITICAL ADMIN CONTROLLERS** (Role 1 Only)

These controllers manage the system itself and should be restricted to Super Admin.

#### **CPanelProfileManagementController**
```csharp
[AuthorizeSession]
[AuthorizeRole("1")]  // Super Admin only
public class CPanelProfileManagementController : Controller
{
	public ActionResult Index() { }              // Role 1: List profiles
	public ActionResult Create() { }             // Role 1: Create role
	[HttpPost]
	public ActionResult Create(/*...*/) { }     // Role 1: Save role
	public ActionResult Edit(int roleid) { }    // Role 1: Edit role
	[HttpPost]
	public ActionResult Edit(/*...*/) { }       // Role 1: Save edits
	public ActionResult Delete(int roleid) { }  // Role 1: Delete role
}
```

**Reason:** Role/profile management affects system-wide permissions; must be Super Admin only.

---

#### **UserController**
```csharp
[AuthorizeSession]
[AuthorizeRole("1,2")]  // Super Admin or Admin
public class UserController : Controller
{
	[HttpGet]
	public ActionResult Index() { }              // Role 1,2: View all users

	[HttpGet]
	public ActionResult Create() { }             // Role 1,2: Create user form

	[HttpPost]
	public ActionResult Create(/*...*/) { }     // Role 1,2: Save new user

	[HttpGet]
	public ActionResult Edit(int userId) { }    // Role 1,2: Edit user

	[HttpPost]
	public ActionResult Edit(/*...*/) { }       // Role 1,2: Save user edits

	[AuthorizeRole("1")]  // Override: Super Admin only
	[HttpPost]
	public ActionResult Delete(int userId) { }  // Role 1 only: Delete user
}
```

**Reason:** User creation/editing affects access control; admin roles only. Deletion is most risky → Role 1 only.

---

#### **ServiceController**
```csharp
[AuthorizeSession]
[AuthorizeRole("1,2")]  // Super Admin or Admin
public class ServiceController : Controller
{
	public ActionResult Index() { }              // Role 1,2: List services
	public ActionResult Create() { }             // Role 1,2: Create service
	[HttpPost]
	public ActionResult Create(/*...*/) { }     // Role 1,2: Save service
	public ActionResult Edit(int serviceId) { }// Role 1,2: Edit service
	[AuthorizeRole("1")]  // Override: Super Admin only
	[HttpPost]
	public ActionResult Delete(int serviceId) {}// Role 1 only: Delete
}
```

**Reason:** Services affect system functionality and customer offerings; admin control needed.

---

#### **BranchsController**
```csharp
[AuthorizeSession]
[AuthorizeRole("1,2")]  // Super Admin or Admin
public class BranchsController : Controller
{
	public ActionResult Index() { }              // Role 1,2: List branches
	public ActionResult Create() { }             // Role 1,2: Create branch
	[HttpPost]
	public ActionResult Create(/*...*/) { }     // Role 1,2: Save branch
	public ActionResult Edit(int branchId) { }  // Role 1,2: Edit branch
	[AuthorizeRole("1")]  // Override: Super Admin only
	[HttpPost]
	public ActionResult Delete(int branchId) { }// Role 1 only: Delete
}
```

**Reason:** Branch management affects organizational structure; admin level control.

---

#### **CurrenciesController**
```csharp
[AuthorizeSession]
[AuthorizeRole("1")]  // Super Admin only
public class CurrenciesController : Controller
{
	public ActionResult Currencies() { }         // Role 1: List currencies
	public ActionResult Create() { }             // Role 1: Create currency
	[HttpPost]
	public ActionResult Create(/*...*/) { }     // Role 1: Save currency
	public ActionResult Edit(int curId) { }     // Role 1: Edit currency
	[HttpPost]
	public ActionResult Edit(/*...*/) { }       // Role 1: Save edits
	[HttpPost]
	public ActionResult Delete(int curId) { }   // Role 1: Delete currency
}
```

**Reason:** Currency configuration affects all transactions; Super Admin only.

---

#### **AccountTypesController**
```csharp
[AuthorizeSession]
[AuthorizeRole("1,2")]  // Super Admin or Admin
public class AccountTypesController : Controller
{
	public ActionResult AccountTypes() { }      // Role 1,2: List types
	public ActionResult Create() { }             // Role 1,2: Create type
	[HttpPost]
	public ActionResult Create(/*...*/) { }     // Role 1,2: Save type
	public ActionResult Edit(int typeId) { }    // Role 1,2: Edit type
	[AuthorizeRole("1")]  // Override: Super Admin only
	[HttpPost]
	public ActionResult Delete(int typeId) { }  // Role 1 only: Delete
}
```

**Reason:** Account types affect product offerings; admin control needed.

---

### 🟡 **HIGH-RISK ADMIN CONTROLLERS** (Role 1,2,3)

These handle customer operations and request processing. Higher-risk actions (reject, delete) should be restricted higher.

#### **ActiveAccountController**
```csharp
[AuthorizeSession]
[AuthorizeRole("1,2,3")]  // All staff
public class ActiveAccountController : Controller
{
	[HttpGet]
	public ActionResult ActiveCustomer() { }    // Role 1,2,3: List requests

	[HttpPost]
	[AuthorizeRole("1,2")]  // Override: Admin+ only
	public ActionResult ActiveCustomerprocess(/*...*/) { }  // Role 1,2: Approve

	[HttpPost]
	[AuthorizeRole("1,2")]  // Override: Admin+ only
	public ActionResult RejectRequest(/*...*/) { }          // Role 1,2: Reject
}
```

**Reason:** Regular staff can view requests, but approval/rejection requires admin judgment.

---

#### **DeActiveAccountController**
```csharp
[AuthorizeSession]
[AuthorizeRole("1,2,3")]  // All staff
public class DeActiveAccountController : Controller
{
	[HttpGet]
	public ActionResult DeActiveCustomer() { }  // Role 1,2,3: View

	[HttpPost]
	[AuthorizeRole("1,2")]  // Override: Admin+ only
	public ActionResult DeActiveCustomerprocess(/*...*/) { }  // Role 1,2: Execute
}
```

**Reason:** Deactivation is high-risk; only admins should authorize.

---

#### **DeleteCustomerController**
```csharp
[AuthorizeSession]
[AuthorizeRole("1,2")]  // Admin+ only (no staff)
public class DeleteCustomerController : Controller
{
	[HttpGet]
	public ActionResult DeleteCustomer() { }    // Role 1,2: List

	[HttpPost]
	[AuthorizeRole("1")]  // Override: Super Admin only
	public ActionResult DeleteCustomerprocess(/*...*/) { }  // Role 1 only: Execute
}
```

**Reason:** Deletion is irreversible; restrict to admin level, execution to Super Admin only.

---

#### **UpdateCustomerController**
```csharp
[AuthorizeSession]
[AuthorizeRole("1,2,3")]  // All staff
public class UpdateCustomerController : Controller
{
	[HttpGet]
	public ActionResult UpdateCustomer() { }    // Role 1,2,3: View

	[HttpPost]
	public ActionResult UpdateCustomerprocess(/*...*/) { }  // Role 1,2,3: Update
}
```

**Reason:** Standard updates allowed for all staff; apply field-level validation (WAPT06-01).

---

#### **CustomerRegistrationController**
```csharp
[AuthorizeSession]
[AuthorizeRole("1,2,3")]  // All staff
public class CustomerRegistrationController : Controller
{
	public ActionResult Registration() { }      // Role 1,2,3: View form

	[HttpPost]
	public ActionResult Registration(/*...*/) { }  // Role 1,2,3: Register

	[HttpPost]
	[AuthorizeRole("1,2")]  // Override: Admin+ only
	public ActionResult ApproveRegistration(/*...*/) { }    // Role 1,2: Approve
}
```

**Reason:** Staff can initiate registrations; admins approve.

---

#### **CustomerRefreshController**
```csharp
[AuthorizeSession]
[AuthorizeRole("1,2,3")]  // All staff
public class CustomerRefreshController : Controller
{
	public ActionResult Refresh() { }           // Role 1,2,3: View

	[HttpPost]
	public ActionResult CustomerRefreshprocess(/*...*/) { }  // Role 1,2,3: Refresh
}
```

**Reason:** Refresh is low-risk operation; all staff allowed.

---

#### **resetCustomerController**
```csharp
[AuthorizeSession]
[AuthorizeRole("1,2,3")]  // All staff
public class resetCustomerController : Controller
{
	[HttpGet]
	public ActionResult ResetCustomer() { }     // Role 1,2,3: View

	[HttpPost]
	public ActionResult ResetCustprocess(/*...*/) { }  // Role 1,2,3: Reset(?)
}
```

**Reason:** If "reset" means soft-reset (temporary disable), all staff. If hard-delete, restrict to admins.
**⚠️ ACTION:** Clarify what "reset" means; adjust restrictions accordingly.

---

### 🟢 **REPORT/VIEW CONTROLLERS** (Role 1,2,3)

These are read-only operations; lower risk. All authenticated staff should have access.

#### **CustomerReportController**
```csharp
[AuthorizeSession]
[AuthorizeRole("1,2,3")]  // All staff
public class CustomerReportController : Controller
{
	public ActionResult CustomersReport() { }   // Role 1,2,3: View report
	public ActionResult ReportDetail(int id) { }// Role 1,2,3: Details
}
```

**Reason:** Reports are read-only; all staff should access for business operations.

---

#### **CustomerTransferReportController**
```csharp
[AuthorizeSession]
[AuthorizeRole("1,2,3")]  // All staff
public class CustomerTransferReportController : Controller
{
	public ActionResult TransferReport() { }    // Role 1,2,3: View report
}
```

**Reason:** Read-only report for all staff reference.

---

#### **ActionsLogController**
```csharp
[AuthorizeSession]
[AuthorizeRole("1,2")]  // Admin+ only
public class ActionsLogController : Controller
{
	public ActionResult ActionsLog() { }        // Role 1,2: Audit log

	[AuthorizeRole("1")]  // Override: Super Admin only
	public ActionResult ExportLog() { }         // Role 1: Export sensitive logs
}
```

**Reason:** Audit logs show sensitive actions; admin+ only. Export (external) is Super Admin only.

---

#### **CustomerAuthorizationController**
```csharp
[AuthorizeSession]
[AuthorizeRole("1,2,3")]  // All staff (or adjust based on your model)
public class CustomerAuthorizationController : Controller
{
	public ActionResult CustomerAuthorization() { }  // View authorizations

	[HttpPost]
	[AuthorizeRole("1,2")]  // Override: Admin+ only
	public ActionResult Approve(/*...*/) { }        // Role 1,2: Approve
}
```

**Reason:** View for all staff, approval restricted to admins.

---

#### **ChangePassController**
```csharp
[AuthorizeSession]
[AuthorizeRole]  // Empty: No role restriction; any logged-in user
public class ChangePassController : Controller
{
	public ActionResult ChangePass() { }        // Any role: Change own password

	[HttpPost]
	public ActionResult ChangePass(/*...*/) { } // Any role: Submit change
}
```

**Reason:** Users should change their own password; no role discrimination needed.

---

#### **MonitoringController**
```csharp
[AuthorizeSession]
[AuthorizeRole("1")]  // Super Admin only
public class MonitoringController : Controller
{
	public ActionResult Monitoring() { }        // Role 1: System monitoring
	public ActionResult SystemHealth() { }      // Role 1: Health check
}
```

**Reason:** System monitoring is sensitive; Super Admin only.

---

### 🔵 **PUBLIC / OPEN CONTROLLERS** (No Role Check)

These controllers don't need role restriction (or handle their own auth).

#### **HomeController**
```csharp
[AuthorizeSession]
// Note: No [AuthorizeRole] → no role restriction
public class HomeController : Controller
{
	public ActionResult Index() { }             // All authenticated users
	public ActionResult Logout() { }            // All authenticated users
}
```

**Reason:** Dashboard accessible to all authenticated users; personalization by role happens inside action.

---

#### **LoginController**
```csharp
public class LoginController : Controller
{
	// ⚠️ NOTE: NO [AuthorizeSession] here!
	// Already handled by AuthorizeSessionAttribute public controller list

	[HttpGet]
	public ActionResult Login() { }             // Public: Pre-login
	[HttpPost]
	public ActionResult Login(/*...*/) { }      // Public: Submit credentials
	public ActionResult Changepassword() { }    // Uses custom auth logic
}
```

**Reason:** Login is public; change password has its own session state logic.

---

#### **ProfileController**
```csharp
[AuthorizeSession]
[AuthorizeRole]  // Empty: No role restriction
public class ProfileController : Controller
{
	public ActionResult ProfileManagement() { }  // All staff: View own profile

	[HttpPost]
	public ActionResult UpdateProfile(/*...*/) { }  // All staff: Edit own profile
}
```

**Reason:** Users manage own profile; no role discrimination (but WAPT06-03 validates ownership).

---

#### **AccountController**
```csharp
[AuthorizeSession]
[AuthorizeRole]  // Empty: No role restriction
public class AccountController : Controller
{
	public virtual PartialViewResult Menu() { }  // All staff: Personalized menu
}
```

**Reason:** Menu is personalized by role inside the method (not in filter).

---

## Implementation Order (Priority)

### Phase 1: CRITICAL (Do First)
1. ✅ CPanelProfileManagementController → `[AuthorizeRole("1")]`
2. ✅ UserController → `[AuthorizeRole("1,2")]` with `[AuthorizeRole("1")]` on Delete
3. ✅ ServiceController → `[AuthorizeRole("1,2")]`
4. ✅ BranchsController → `[AuthorizeRole("1,2")]`

### Phase 2: HIGH-RISK (Do Second)
5. DeleteCustomerController → `[AuthorizeRole("1")]`
6. ActionsLogController → `[AuthorizeRole("1,2")]`
7. ActiveAccountController → Partial: class `[AuthorizeRole("1,2,3")]`, methods override

### Phase 3: STANDARD (Do Third)
8. CurrenciesController, AccountTypesController, etc. (from phase 1)
9. CustomerReportController, CustomerTransferReportController (read-only)

### Phase 4: FLEXIBLE (Do Last / As Needed)
10. HomeController, ProfileController (already have basic session check)
11. LoginController (already public)

---

## Verification Checklist

After applying roles to each controller:

- [ ] **No orphaned actions** — Every action inherits class role or has explicit override
- [ ] **No overly permissive** — `[AuthorizeRole("1,2,3")]` only where justified
- [ ] **No overly restrictive** — Officers (role 3) can still view/process operational requests
- [ ] **Delete operations** — Always restricted above regular operations
- [ ] **Admin-only** — CPanelProfileManagement, Currencies, Monitoring
- [ ] **Admin-plus** — User, Service, Branch, ActiveAccount (approve/reject)
- [ ] **All-staff** — Reports, Registration, CustomerRefresh, UpdateCustomer
- [ ] **Logged-in only** — ChangePass, ProfileController, HomeController

---

## Testing Strategy

### Test Each Role

1. **Log in as Role 1 (Super Admin)**
   - Should access: All CRITICAL + HIGH-RISK + STANDARD controllers
   - Should NOT see: Error pages

2. **Log in as Role 2 (Admin)**
   - Should access: Most HIGH-RISK + STANDARD controllers
   - Should get 403: Delete-only actions, CRITICAL admin actions

3. **Log in as Role 3 (Officer)**
   - Should access: Reports, Update, Refresh, Registration
   - Should get 403: Any admin/delete action

4. **Not logged in**
   - Should redirect to Login: All protected controllers

---

**READY FOR FASE 2: WAPT03-02**

Next step: Apply these role restrictions to all controllers.
