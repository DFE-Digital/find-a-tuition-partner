Feature: User handles cookies
  Scenario: cookies page url is '/cookies'
    Given a user has started the 'Find a tuition partner' journey
    When the 'view cookies' is selected
    Then the page URL ends with '/cookies'

  Scenario: cookies page title is 'Cookies'
    Given a user has started the 'Find a tuition partner' journey
    When the 'view cookies' is selected
    Then the page's title is 'Cookies'

  Scenario: user clicks service name on cookies page
    Given a user has started the 'Find a tuition partner' journey
    When the 'view cookies' is selected
    And they click the 'Find a tuition partner' service name link
    Then they will be taken to the 'Find a tuition partner' journey start page

  Scenario: The cookies banner is displayed when application is accessed for first time
    Given a user has started the 'Find a tuition partner' journey
    Then the cookies banner is displayed

  Scenario: User accepts cookies
    Given a user has started the 'Find a tuition partner' journey
    When cookies are accepted
    Then user session is tracked
    And the banner disappears
    And cookie '.FindATuitionPartner.Consent' is added with value 'True'

  Scenario: User rejects cookies
    Given a user has started the 'Find a tuition partner' journey
    When cookies are rejected
    Then user session is not tracked
    And the banner disappears
    And cookie '.FindATuitionPartner.Consent' is added with value 'False'

  Scenario: 'View cookies' is selected
    Given a user has started the 'Find a tuition partner' journey
    When the 'view cookies' is selected
    Then the 'view cookies' page is loaded

  Scenario: The cookies banner is not displayed again after accepting cookies
    Given a user accesses a service page after accepting cookies
    Then the cookie banner is not displayed

  Scenario: The cookies banner is not displayed again after rejecting cookies
    Given a user accesses a service page after rejecting cookies
    Then the cookie banner is not displayed

  Scenario: The cookie banner is not displayed when 'cookie statement' is selected when entering for the first time
    Given a user has started the 'Find a tuition partner' journey
    When the 'cookies' is selected from footer 
    Then the 'view cookies' page is loaded
    And the cookie banner is not displayed

  Scenario: The cookie banner is not displayed when 'cookie statement' is selected when cookies have already been set
    Given a user accesses a service page after accepting cookies
    When the 'cookies' is selected from footer
    Then the 'view cookies' page is loaded
    And the cookie banner is not displayed

  Scenario: User selects cookie statement and then selects the 'privacy policy' link on that page
    Given a user has started the 'Find a tuition partner' journey
    When the 'cookies' is selected from footer 
    Then the 'view cookies' page is loaded
    And the privacy policy is accessible in a new tab

  Scenario: User returns to previous page after setting preference
    Given the success banner has been displayed
    When the link to previous page is clicked
    Then the previous page is displayed correctly

  Scenario: Allow users to opt in once they have reached the cookies page for first time
    Given the 'view cookies' page is displayed
    And nothing is selected
    When a user opts-in 
    And Saves Changes
    Then opt-in is selected
    And a Success Banner is displayed
    And the link to previous page is clicked
    And user session is tracked

  Scenario: Allow users to opt out once they have reached the cookies page for first time
    Given the 'view cookies' page is displayed
    And nothing is selected
    When a user opts-out 
    And Saves Changes
    Then opt-out is selected
    And a Success Banner is displayed
    And user session is not tracked

  Scenario: Error banner appears if no option is selected
    Given the 'view cookies' page is displayed
    And nothing is selected
    When Saves Changes
    Then the error banner is displayed

  Scenario: The cookie '.FindATuitionPartner.Mvc.CookieTempDataProvider' is added when a user has reached the search page
    Given the search result page is displayed
    Then cookie '.FindATuitionPartner.Mvc.CookieTempDataProvider' is added with value 'null'

  Scenario: User Select Accept Option from banner Should Stay on Same Page 
    Given a user has arrived on the funding and reporting page
    When cookies are accepted
    And the user redirected to funding page

  Scenario: User Select Reject Option from banner Should Stay on Same Page 
    Given a user has arrived on the funding and reporting page
    When cookies are rejected
    And the user redirected to funding page
