# Practice 1: JWT Token Decode Worksheet

**Day 12 -- OIDC Overview and Claims-Based Identity**
**Time:** 15 minutes | **Type:** Guided

---

## Instructions

1. Open [jwt.io](https://jwt.io) in your browser
2. Open the file `valid-token.txt` and copy **only the first line** (the long encoded string before the `---`)
3. Paste the token into the "Encoded" field on jwt.io
4. Answer Part A questions using the decoded token
5. Then open `expired-token.txt`, copy the first line, paste it into jwt.io, and answer Part B

---

## Part A: Valid Token

Paste the valid token into jwt.io and answer:

**1. What signing algorithm does this token use?**

> _______________________________________________

**2. Who issued this token? (Look at the `iss` claim)**

> _______________________________________________

**3. What is the user's unique identifier? (Look at the `sub` claim)**

> _______________________________________________

**4. What is the user's email address?**

> _______________________________________________

**5. What roles does this user have? (Look for the custom roles claim)**

> _______________________________________________

**6. When was this token issued? (Convert the `iat` Unix timestamp to a human-readable date)**

Hint: Use [epochconverter.com](https://www.epochconverter.com) to convert.

> _______________________________________________

**7. When does this token expire? (Convert the `exp` Unix timestamp)**

> _______________________________________________

**8. What is the intended audience for this token? (Look at the `aud` claim)**

> _______________________________________________

**9. List all the custom claims (claims that use a namespace URI like `https://campus-store.com/claims/...`):**

> _______________________________________________
>
> _______________________________________________
>
> _______________________________________________

---

## Part B: Expired Token

Clear the jwt.io decoder. Paste the expired token and answer:

**10. What is different about this token compared to the valid one?**

> _______________________________________________

**11. Should your application accept this token? Why or why not?**

> _______________________________________________
>
> _______________________________________________

---

## Bonus: Tamper Test

Open `tamper-test-tokens.txt`. The Secret box on jwt.io must hold `a-string-secret-at-least-256-bits-long` (jwt.io fills it in by default).

1. Paste the **ORIGINAL** line into jwt.io. Note what the signature indicator says.
2. Paste the **TAMPERED** line. Only the `name` claim changed. What does the signature indicator say now?

> _______________________________________________

3. Why does modifying the payload invalidate the signature?

> _______________________________________________
