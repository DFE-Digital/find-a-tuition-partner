Feature: Tuition Partner shortlist

  Scenario: User can add a TP to their shortlist from the results page
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    When they add 'Action Tutoring' to their shortlist on the results page
    Then 'Action Tutoring' is marked as shortlisted on the results page
    And the shortlist shows as having 1 entries on the results page

  Scenario: User can add multiple TPs to their shortlist from the results page
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    When they add 'Action Tutoring' to their shortlist on the results page
    And they add 'm2r Education' to their shortlist on the results page
    Then 'Action Tutoring' is marked as shortlisted on the results page
    And 'm2r Education' is marked as shortlisted on the results page
    And the shortlist shows as having 2 entries on the results page

    Scenario: User can add lots of TPs to their shortlist in quick succession from the results page
        Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
        When they programmatically add the first 20 results to their shortlist on the results page
        Then the shortlist shows as having 20 entries on the results page

  Scenario: User can remove a TP from their shortlist from the results page
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    And they add 'Action Tutoring' to their shortlist on the results page
    When they remove 'Action Tutoring' from their shortlist on the results page
    Then 'Action Tutoring' is not marked as shortlisted on the results page
    And the shortlist shows as having 0 entries on the results page

  Scenario: User goes straight to their empty shortlist page
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    When they choose to view their shortlist from the results page
    Then there are 0 entries on the shortlist page

  Scenario: Shortlist back button takes the user back to the search results
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    And they choose to view their shortlist from the results page
    When they click 'Back'
    Then the search results are displayed

  Scenario: User views their shortlisted TPs on the shortlist page
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    And they add 'Action Tutoring' to their shortlist on the results page
    And they add '3D Recruit Ltd' to their shortlist on the results page
    When they choose to view their shortlist from the results page
    Then there are 2 entries on the shortlist page
    And the heading caption is 'Tuition partners for Stockport'
    And 'Action Tutoring' is entry 1 on the shortlist page
    And '3D Recruit Ltd' is entry 2 on the shortlist page

  Scenario: User changes their filters to exclude a shortlisted TP
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    And they add 'Action Tutoring' to their shortlist on the results page
    And they add '3D Recruit Ltd' to their shortlist on the results page
    When they click on the option heading for 'Key stage 4'
    And they select subject 'key-stage-4-humanities'
    Then the shortlist shows as having 2 entries on the results page

  Scenario: User changes their postcode to exclude a shortlisted TP
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    And they add 'Action Tutoring' to their shortlist on the results page
    And they add '3D Recruit Ltd' to their shortlist on the results page
    When they enter 'TN22 2BL' as the school's postcode
    And they click 'Search'
    Then the shortlist shows as having 2 entries on the results page
    And they choose to view their shortlist from the results page
    And there are 1 entries on the shortlist page
    And the heading caption is 'Tuition partner for East Sussex'
    And '3D Recruit Ltd' is entry 1 on the shortlist page
    And 'Action Tutoring' is entry 1 on the not available list on the shortlist page

  Scenario: The shortlist displays the expected data for the search area
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'W1W 7RT'
    And they add 'Reeson Education' to their shortlist on the results page
    When they choose to view their shortlist from the results page
    Then there are 1 entries on the shortlist page
    And entry 1 on the shortlist is the row 'Reeson Education', '1, 2, 3 and 4', 'English, Maths, Science, Humanities and Modern Foreign Languages', 'In School, Online', 'From £10'
    And they click 'Back'
    And they enter 'TN22 2BL' as the school's postcode
    And they click 'Search'
    And they choose to view their shortlist from the results page
    And entry 1 on the shortlist is the row 'Reeson Education', '1, 2, 3 and 4', 'English, Maths, Science, Humanities and Modern Foreign Languages', 'Online', 'From £10'

  Scenario: User views TP details from the shortlist
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    And they add 'Action Tutoring' to their shortlist on the results page
    And they choose to view their shortlist from the results page
    When they choose to view the 'Action Tutoring' details from the shortlist
    Then the back link's text is 'Back to shortlist'
    And the heading caption is 'Shortlisted tuition partner for Stockport'
    And they click 'Back'
    And they will be taken to the 'Shortlist' page

  Scenario: User sorts shortlist table by Name
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    And they add 'Reeson Education' to their shortlist on the results page
    And they add 'Action Tutoring' to their shortlist on the results page
    And they add '3D Recruit Ltd' to their shortlist on the results page
    When they choose to view their shortlist from the results page
    Then there are 3 entries on the shortlist page
    Then they choose to sort the shortlist by name
    And '3D Recruit Ltd' is entry 1 on the shortlist page
    And 'Action Tutoring' is entry 2 on the shortlist page
    And 'Reeson Education' is entry 3 on the shortlist page
    Then they choose to sort the shortlist by name
    And 'Reeson Education' is entry 1 on the shortlist page
    And 'Action Tutoring' is entry 2 on the shortlist page
    And '3D Recruit Ltd' is entry 3 on the shortlist page

  Scenario: User sorts shortlist table by Price
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    And they add 'Reeson Education' to their shortlist on the results page
    And they add 'Action Tutoring' to their shortlist on the results page
    And they add '3D Recruit Ltd' to their shortlist on the results page
    When they choose to view their shortlist from the results page
    Then there are 3 entries on the shortlist page
    Then they choose to sort the shortlist by price
    And 'Reeson Education' is entry 1 on the shortlist page
    And '3D Recruit Ltd' is entry 2 on the shortlist page
    And 'Action Tutoring' is entry 3 on the shortlist page
    Then they choose to sort the shortlist by price
    And 'Action Tutoring' is entry 1 on the shortlist page
    And '3D Recruit Ltd' is entry 2 on the shortlist page
    And 'Reeson Education' is entry 3 on the shortlist page

  Scenario: User removes single item from shortlist
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    And they add 'Reeson Education' to their shortlist on the results page
    And they add 'Action Tutoring' to their shortlist on the results page
    And they add '3D Recruit Ltd' to their shortlist on the results page
    When they choose to view their shortlist from the results page
    Then there are 3 entries on the shortlist page
    And 'Reeson Education' is entry 1 on the shortlist page
    And 'Action Tutoring' is entry 2 on the shortlist page
    And '3D Recruit Ltd' is entry 3 on the shortlist page
    Then they choose to remove entry 2 on the shortlist page
    Then there are 2 entries on the shortlist page
    And 'Reeson Education' is entry 1 on the shortlist page
    And '3D Recruit Ltd' is entry 2 on the shortlist page
    Then they click 'Back'
    And the search results are displayed
    Then they choose to view their shortlist from the results page
    Then there are 2 entries on the shortlist page
    And 'Reeson Education' is entry 1 on the shortlist page
    And '3D Recruit Ltd' is entry 2 on the shortlist page

  Scenario: User clears full shortlist then cancel
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    And they add 'Reeson Education' to their shortlist on the results page
    And they add 'Action Tutoring' to their shortlist on the results page
    And they add '3D Recruit Ltd' to their shortlist on the results page
    When they choose to view their shortlist from the results page
    Then there are 3 entries on the shortlist page
    And 'Reeson Education' is entry 1 on the shortlist page
    And 'Action Tutoring' is entry 2 on the shortlist page
    And '3D Recruit Ltd' is entry 3 on the shortlist page
    Then they choose to click on clear shortlist link
    And they are taken to the clear shortlist confirmation page
    Then they click the cancel link
    Then there are 3 entries on the shortlist page

  Scenario: User clears full shortlist
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    And they add 'Reeson Education' to their shortlist on the results page
    And they add 'Action Tutoring' to their shortlist on the results page
    And they add '3D Recruit Ltd' to their shortlist on the results page
    When they choose to view their shortlist from the results page
    Then there are 3 entries on the shortlist page
    And 'Reeson Education' is entry 1 on the shortlist page
    And 'Action Tutoring' is entry 2 on the shortlist page
    And '3D Recruit Ltd' is entry 3 on the shortlist page
    Then they choose to click on clear shortlist link
    And they are taken to the clear shortlist confirmation page
    Then they click confirm button
    Then there are 0 entries on the shortlist page

  Scenario: Adding or removing TP to shortlist from search results page should be reflected on the TP details page
    Given a user has arrived on the 'Search results' page for 'Key stage 1 English' for postcode 'sk11eb'
    And total amount of shortlisted TPs is 0
    And 'Seven Springs Education' checkbox is unchecked
    Then 'Seven Springs Education' name link is clicked
    And 'Seven Springs Education' checkbox is unchecked on its detail page
    And the LA label text is 'Tuition partner for Stockport'
    Then they click 'Back'
    Then they add 'Seven Springs Education' to their shortlist on the results page
    And total amount of shortlisted TPs is 1
    Then 'Seven Springs Education' name link is clicked
    And  'Seven Springs Education' checkbox is checked on its detail page
    And the LA label text is 'Shortlisted tuition partner for Stockport'
    Then they click 'Back'
    Then they remove 'Seven Springs Education' from their shortlist on the results page
    And total amount of shortlisted TPs is 0
    Then 'Seven Springs Education' name link is clicked
    And 'Seven Springs Education' checkbox is unchecked on its detail page
    And the LA label text is 'Tuition partner for Stockport'
    
  Scenario: Removing last shortlisted TP from TP details page is reflected in shortlist page
    Given a user has arrived on the 'Search results' page for 'Key stage 2 English' for postcode 'SK1 1EB'
    And they add 'Reeson Education' to their shortlist on the results page
    When they choose to view their shortlist from the results page
    Then there is 1 entry on the shortlist page
    And 'Reeson Education' is entry 1 on the shortlist page
    Then 'Reeson Education' name link is clicked
    And 'Reeson Education' is removed from the shortlist
    Then they click Back to go back to the shortlist page
    And the shortlist page displays 'You don’t have any shortlisted tuition partners.'
    