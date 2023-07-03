
<body style="font-family: 'Poppins', sans-serif;">
  <h3>
    We Had To Finish This Project At 1 July So Done All Your Work & Also Those Team Members Who Made the Documentation Should Start Working From Now!
  </h3>
  
  <h2>All Team Members There is the Database File for Implementation of This Project:</h2>
  <a href="https://cutt.ly/JwulXTxb">https://cutt.ly/JwulXTxb</a>
  <h2>There is the Tutorial Video of How You Can Import The Database into SQL Server:</h2>


https://github.com/TheMehrozKhan/FlavourFusion/assets/103773815/6f0b5df6-52a5-4be0-a44c-d730f8593a8b


  <h2>There are General Steps or Precautions For Operating The Project:</h2>
  <h3>1) Run This Query In SQL Server After Importing The Database:</h3>
  <code>INSERT INTO Tbl_Membership_Plans (plan_id,plan_name, price, duration)
    VALUES (1, 'Standard', 35, 6);
    INSERT INTO Tbl_Membership_Plans (plan_id,plan_name, price, duration)
    VALUES (2, 'Premium', 50, 12);
    INSERT INTO Tbl_Membership_Plans (plan_id, plan_name, price, duration)
    VALUES (3, 'Free', 0, 1);</code>

  <h3>2) Insert This Membership Status Code in The Tbl_User for Fetching The Details of Membership in User Model To Display it in User Profile:</h3>
  <code>public string MembershipStatus { get; set; }</code>

<h3>3) Insert this Code in The Model of Tbl_Recipe for Fetching the Recipe Direction & Video:</h3> 
<code>using System.Web.Mvc;</code>
<code>  
  [AllowHtml]
        public string recipe_direction { get; set; }
        [AllowHtml]
        public string recipe_tutorial_video { get; set; } </code>


<h3>4) Don't Forget to Install The SendGrid for Sending The Mail To The User When The Admin Declared User as Winner:</h3>
<code>Install-Package SendGrid</code>

<h2>Tutorial Video of How You Can Solve MembershipStatus Error:</h2>
https://github.com/TheMehrozKhan/FlavourFusion/assets/103773815/8e0929ba-7af0-44a6-b60c-7d0abe35df72





  <h2>There is the Complete Category Data For The Project:</h2>
  <a href="https://bit.ly/3PlM2sA">https://bit.ly/3PlM2sA</a>
</body>
