Feature: Tuition partner details mobile view page tests
  Scenario: Subject list is bullet pointed on search results page in mobile phone view
    Given a user has arrived on the 'Search results' page
    And a user is using a 'phone'
    Then the subject list is bullet pointed

  Scenario: Subject list is bullet pointed on tuition partner page in mobile phone view
    Given a user has arrived on the 'Tuition Partner' page for tp name 11
    And a user is using a 'phone'
    Then the subject list is bullet pointed

  Scenario: Subject list is not bullet pointed on search results page in tablet and above view
    Given a user has arrived on the 'Search results' page
    And a user is using a 'tablet'
    Then the subject list is not bullet pointed

  Scenario: Subject list is not bullet pointed on tuition partner page in tablet and above view
    Given a user has arrived on the 'Tuition Partner' page for tp name 11
    And a user is using a 'tablet'
    Then the subject list is not bullet pointed

  Scenario: Search results page heading is 'Find a tuition partner' in tablet and above view
    Given a user has arrived on the 'Search results' page
    And a user is using a 'tablet'
    Then the search results page heading is 'Your options for choosing a tuition partner'

  Scenario: Search results page heading is 'Search results' in mobile phone view
    Given a user has arrived on the 'Search results' page
    And a user is using a 'phone'
    Then the search results page heading is 'Your options now'

  Scenario: Search results filter heading is 'Filter results' in tablet and above view
    Given a user has arrived on the 'Search results' page
    And a user is using a 'tablet'
    Then the search results filter heading is displayed
    And the search results filter heading is 'Filter results'
    And the overlay search results filter heading is not displayed

  Scenario: Search results page shows filters and results in tablet and above view
    Given a user has arrived on the 'Search results' page
    And a user is using a 'tablet'
    Then the search filters, postcode and results sections are all displayed

  Scenario: Search results page does not show filters in mobile phone view
    Given a user has arrived on the 'Search results' page
    And a user is using a 'phone'
    Then the search filters are not displayed

  Scenario: Search results page does not have show filters button in tablet and above view
    Given a user has arrived on the 'Search results' page
    And a user is using a 'tablet'
    Then the show filters button is not displayed

  Scenario: Search results page does has show filters button in mobile phone view
    Given a user has arrived on the 'Search results' page
    And a user is using a 'phone'
    Then the show filters button is displayed

  Scenario: Clicking show filters overlays filters in mobile phone view
    Given a user has arrived on the 'Search results' page
    And a user is using a 'phone'
    When they click 'Show filters'
    Then the search filters are displayed

  Scenario: Mobile filters overlay displays filters heading, results count and subjects header
    Given a mobile user has opened the mobile filters overlay
    Then the overlay search results filter heading is 'Filters'
    And the overlay search results filter heading has the results count
    And the overlay search results filter heading has the subjects header

  Scenario: Mobile filters overlay displays return to results link and show search results button
    Given a mobile user has opened the mobile filters overlay
    Then the return to results link is displayed
    And the show search results button is displayed

  Scenario: Selecting return to results link cancels filter changes
    Given a mobile user has opened the mobile filters overlay
    When they select subject 'Key stage 1 Maths'
    And they select the 'Return to results' link
    Then subject 'KeyStage1-Maths' is no longer selected
    And the search filters are not displayed

  Scenario: Selecting show search results button applies filter changes
    Given a mobile user has opened the mobile filters overlay
    When they select subject 'Key stage 1 Maths'
    And they select the 'Show search results' button
    Then subject 'KeyStage1-Maths' is selected on the filter
    And the search filters are not displayed

  Scenario: Type of tuition page mobile structure
    Given a user begins journey from a mobile
    Then the postcode is edited in the start page
    And a user has arrived on the 'Which subjects' page for 'Key stage 3, Key stage 4'
    Then they select subjects for the key stages
    When they click 'Continue'
    And they will be taken to the type of tuition page
    Then the correct options will display
    When user clicks the button with text 'In School'
    And they click 'Continue'
    Then they will be taken to the 'Search Results' page
    And the filter results show the expected selection

  Scenario: Make an enquiry journey
    Given a user has arrived on the 'Search results' page for 'Key stage 1 English'
    When they click 'Start now'
    And they click 'Continue' on enquiry
    Then they enter a valid email address
    When they click 'Continue'
    Then they are redirected to the enquiry question page

  Scenario: Tuition partner details are not displayed when no postcode entered
    Given a mobile user has opened the mobile filters overlay
    Then a user has arrived on the 'Search results' page without subjects for postcode ''
    And a user is using a 'phone'
    Then they will see 'Enter a postcode' as an error message for the 'postcode'

  Scenario: Tuition plan question shows correct error when no input provided
    Given user navigates to the first enquiry question
    And a user is using a 'phone'
    When they click 'Continue'
    Then they will see 'Enter the type of tuition plan that you need' as an error message for the 'no input error'

  Scenario: Tuition plan question shows correct warning when reaching close to Max char limit
    Given user navigates to the first enquiry question
    And a user is using a 'phone'
    When they type '7600' characters for question 1
    Then the warning should be displayed showing they have '2400' characters left
    When they type '11000' characters for question 1
    Then the warning should be displayed showing they are over by '1000' characters

  Scenario: Tuition plan second question shows correct warning when reaching close to Max char limit
    Given user navigates to the first enquiry question
    And a user is using a 'phone'
    When they enter an answer for tuition plan
    And they click 'Continue'
    When they type '7600' characters for question 2
    Then the warning should be displayed showing they have '2400' characters left
    When they type '11000' characters for question 2
    Then the warning should be displayed showing they are over by '1000' characters

  Scenario: User can skip second and third question in Enquiry Builder and will show as Not specified on check your answers
    Given user navigates to the first enquiry question
    And a user is using a 'phone'
    When they enter an answer for tuition plan
    And they click 'Continue'
    Then they click 'Continue'
    And they click 'Continue'
    Then the text by the second and third questions is 'Not specified'

  Scenario: Tuition plan third question shows correct warning when reaching close to Max char limit
    Given user navigates to the first enquiry question
    And a user is using a 'phone'
    When they enter an answer for tuition plan
    And they click 'Continue'
    Then they enter an answer for SEND requirements
    And they click 'Continue'
    When they type '7600' characters for question 3
    Then the warning should be displayed showing they have '2400' characters left
    When they type '11000' characters for question 3
    Then the warning should be displayed showing they are over by '1000' characters

  Scenario: Navigate to the Check Your Answers page
    Given a user has started the 'Find a tuition partner' journey
    And a user is using a 'phone'
    When user has journeyed forward to the check your answers page
    Then they are redirected to the check your answers page

  Scenario: Verify Check Your Answers page content
    Given a user has started the 'Find a tuition partner' journey
    When user has journeyed forward to the check your answers page
    Then they are redirected to the check your answers page
    Then the Check Your Answers page displays the following:
      | Section Name            | Expected Content                                |
      | Key stages and subjects | Displays Key Stages and subjects table          |
      | Type of Tuition         | Any                                             |
      | Email Address           | simulate-delivered@notifications.service.gov.uk |
      | Tuition Plan            | enquiry                                         |
      | SEND Support            | enquiry                                         |
      | Other Considerations    | enquiry                                         |


  Scenario: Change selections for Key Stages and Subjects
    Given a user has started the 'Find a tuition partner' journey
    And user has journeyed forward to the check your answers page
    When the user clicks the change button '1'
    Then the user is taken back to the key-stage page to change their selection
    And they will see 'Key stage 1, Key stage 2' selected
    When they select Key stage 3
    And they click 'Continue'
    Then the user is taken to the subjects page to change their selection
    When they select science for all three key stages
    And they click 'Continue'
    When they are redirected to the check your answers page
    Then the Check Your Answers page displays the following with the key stage and subjects updates:
      | Section Name            | Expected Content                                |
      | Key stages and subjects | Displays Key Stage and Subjects table           |
      | Type of Tuition         | Any                                             |
      | Email Address           | simulate-delivered@notifications.service.gov.uk |
      | Tuition Plan            | enquiry                                         |
      | SEND Support            | enquiry                                         |
      | Other Considerations    | enquiry                                         |

  Scenario: Change selections for Type of Tuition
    Given a user has started the 'Find a tuition partner' journey
    And user has journeyed forward to the check your answers page
    When the user clicks the change button '2'
    Then the user is taken back to the type of tuition page
    And they select 'Online'
    And they click 'Continue'
    Then they are redirected to the check your answers page
    Then the Check Your Answers page displays the following with the type of tuition update:
      | Section Name            | Expected Content                                |
      | Key stages and subjects | Displays Key Stage and Subjects table           |
      | Type of Tuition         | Online                                          |
      | Email Address           | simulate-delivered@notifications.service.gov.uk |
      | Tuition Plan            | enquiry                                         |
      | SEND Support            | enquiry                                         |
      | Other Considerations    | enquiry                                         |

  Scenario: Change selections for email address
    Given a user has started the 'Find a tuition partner' journey
    And user has journeyed forward to the check your answers page
    When the user clicks the change button '3'
    Then the user is taken back to the email address page
    And they enter another email address
    And they click 'Continue'
    Then they are redirected to the check your answers page
    Then the Check Your Answers page displays the following with the email address update:
      | Section Name            | Expected Content                      |
      | Key stages and subjects | Displays Key Stage and Subjects table |
      | Type of Tuition         | Any                                   |
      | Email Address           | new@new.com                           |
      | Tuition Plan            | enquiry                               |
      | SEND Support            | enquiry                               |
      | Other Considerations    | enquiry                               |

  Scenario: Change selections for tuition questions
    Given a user has started the 'Find a tuition partner' journey
    And user has journeyed forward to the check your answers page
    When the user clicks the change button '4'
    Then they are redirected to the enquiry question page
    When they type '5' characters for question 1
    And they click 'Continue'
    Then they are redirected to the check your answers page
    When the user clicks the change button '5'
    Then they are redirected to the SEND requirements page
    When they type '5' characters for question 2
    And they click 'Continue'
    Then they are redirected to the check your answers page
    When the user clicks the change button '6'
    Then they are redirected to the other requirements page
    When they type '0' characters for question 3
    And they click 'Continue'
    Then they are redirected to the check your answers page
    Then the Check Your Answers page displays the following with the email address update:
      | Section Name            | Expected Content                                |
      | Key stages and subjects | Displays Key Stage and Subjects table           |
      | Type of Tuition         | Any                                             |
      | Email Address           | simulate-delivered@notifications.service.gov.uk |
      | Tuition Plan            | aaaaa                                           |
      | SEND Support            | aaaaa                                           |
      | Other Considerations    | Not specified                                   |



  Scenario: Clear search filters in search page and attempt to make an enquiry
    Given user navigates to check your answers unselecting filter results
    And a user is using a 'phone'
    And they select terms and conditions
    Then they click send enquiry
    Then the correct error message should display for no keystage and subjects selected
