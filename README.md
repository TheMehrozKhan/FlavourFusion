<style>
  @import url('https://fonts.googleapis.com/css2?family=Poppins&display=swap');

  * {
    font-family: 'Poppins', sans-serif;
  }

</style>
<h2>All Team Members There is the Database File for Implementation of This Project:<h2>
https://bit.ly/3XjjMZF

<h2> There is the General Steps or Precautions For Operating The Project: </h2>
<h3> 1) Run This Query In SQL Server After Importing The Database: </h3>
INSERT INTO Tbl_Membership_Plans (plan_id,plan_name, price, duration)
VALUES (1, 'Standard', 35, 6);
INSERT INTO Tbl_Membership_Plans (plan_id,plan_name, price, duration)
VALUES (2, 'Premium', 50, 12);
INSERT INTO Tbl_Membership_Plans (plan_id, plan_name, price, duration)
VALUES (3, 'Free', 0, 1);

<h3> 2) Insert This Membership Status Code in The Tbl_User for Fetching The Details of Membership in User Model To Display it in User Profile: </h3>
public string MembershipStatus { get; set; }

<h2> There is the Complete Data For The Project: </h2>
https://bit.ly/3PlM2sA
