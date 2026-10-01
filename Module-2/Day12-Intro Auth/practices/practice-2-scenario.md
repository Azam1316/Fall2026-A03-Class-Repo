# Practice 2: Claims Design -- Hospital Patient Portal

**Day 12 -- OIDC Overview and Claims-Based Identity**
**Time:** 20 minutes | **Type:** Pair Work

---

## Scenario

You are designing the authentication and authorization system for **MedView**, a hospital patient portal. The portal serves four types of users with different access needs.

### User Access Requirements

| User Type | Can View | Can Do |
|-----------|----------|--------|
| **Doctor** | All patients in their department, full medical records, lab results | Create/edit treatment plans, order lab tests, discharge patients |
| **Nurse** | Patients in their assigned unit, vital signs, medication schedules | Update vital signs, administer medications, flag alerts |
| **Patient** | Their own records only (no other patients) | View personal records, request appointments, message their doctor |
| **Admin** | Staff directory, system logs (no patient data) | Add/remove staff accounts, assign departments, manage roles |

### Additional Business Rules

- A doctor in Cardiology should NOT see patients in Orthopedics
- A nurse on ICU should NOT access records from Ward 3A
- Patients can only see their own data -- never another patient's
- Admins manage the system but have zero access to patient medical data

---

## Your Task

Work with your partner to design the claims-based identity model for MedView.

### Part 1: Roles

What roles does your system need? List them.

| Role | Who Gets This Role |
|------|-------------------|
| | |
| | |
| | |
| | |

### Part 2: Custom Claims

Beyond the standard OIDC claims (`sub`, `name`, `email`), what custom claims does your system need?

Remember: custom claims should use a namespace URI (e.g., `https://medview.com/claims/...`).

| Custom Claim | Example Value | Purpose |
|-------------|---------------|---------|
| | | |
| | | |
| | | |
| | | |

### Part 3: Data Each Role Sees

For each role you listed in Part 1, describe in plain language **what data that role should be able to see** — and any conditions on that access.

> *Note: in this practice you stop at the design layer (intended access). The C# code that enforces these rules — called *authorization policies* — is introduced on Day 16.*

**Example:** "Doctor: full medical records for any patient whose `department` claim matches the doctor's `department` claim."

**Role 1 (___________):** _______________________________________________

> _______________________________________________

**Role 2 (___________):** _______________________________________________

> _______________________________________________

**Role 3 (___________):** _______________________________________________

> _______________________________________________

**Role 4 (___________):** _______________________________________________

> _______________________________________________

---

## Discussion Questions (At 15-min mark, be ready to share)

1. Could you implement this system using only roles (no custom claims)? What problems would that cause?
2. What happens if a doctor is also an administrator? How does your design handle dual roles?
3. How would you ensure a patient can ONLY see their own records and not another patient's?
