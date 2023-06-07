Feature: User can travel forwards and backwards
  Scenario: User can move forward in the application to the end of the journey
    Given a user has started the 'Find a tuition partner' journey
    Then user has journeyed forward to a selected tuition partner page

  Scenario: User can travel back to the begining of journey
    Given a user has started the 'Find a tuition partner' journey
    And user has journeyed forward to a selected tuition partner page
    Then they will be journey back to the page they started from

  Scenario: User can travel back to the beginning of journey without loss of data
    Given a user has started the 'Find a tuition partner' journey
    And user has journeyed forward to a selected tuition partner page
    And they will be journey back to the page they started from
    Then the key stages are correct in the key stages page
    And the subjects are correct in the subjects page
    And the filter selections are correct in the search results page

  Scenario: User can travel back to the begining of journey and edit previous selections
    Given a user has started the 'Find a tuition partner' journey
    And user has journeyed forward to a selected tuition partner page
    And they will be journey back to the page they started from
    When the postcode is edited in the start page
    And the key stages are edited in the key stages page
    And the subjects are edited in the subjects page after key stage has been edited
    And the user selects tuition setting 'face-to-face'
    And they click 'Continue'
    Then the filter selections are correct in the search results page with the edited selections

  Scenario: User can search by postcode SK1 1EB to see Action Tutoring TP with filtered tuition (online only) types then search by new postcode NR1 1BD that excludes Action Tutoring and then find Action Tutoring via find all will show unfiltered tuition settings (Online and School)
    Given a user has started the 'Find a tuition partner' journey
    And user has entered 'SK1 1EB' and journeyed forward to the 'Action Tutoring' tuition partner page
    And they see the tuition settings 'Online'
    And they journey back to the search page
    And they update the postcode on the search page to 'NR1 1BD' and go to a selected tuition partner page
    And they will be journey back to the page they started from
    When they click the 'Check the full list of quality-assured tuition partners.'link
    And they select the 'Action Tutoring' tuition partner page
    Then they see the tuition settings 'Face-to-face, Online'