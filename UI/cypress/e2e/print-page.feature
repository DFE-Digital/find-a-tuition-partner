Feature: User can print relevant pages using a 'Print this page' link
  Scenario: start page does not have a 'Print this page' link
    Given a user has started the 'Find a tuition partner' journey
    Then the 'Print this page' link is not displayed

  Scenario: contact us page does not have a 'Print this page' link
    Given a user has arrived on the contact us page
    Then the 'Print this page' link is not displayed

  Scenario: accessibility page does not have a 'Print this page' link
    Given a user has arrived on the accessibility page
    Then the 'Print this page' link is not displayed

  Scenario: cookies page does not have a 'Print this page' link
    Given a user has arrived on the cookies page
    Then the 'Print this page' link is not displayed

  Scenario: privacy notice page does not have a 'Print this page' link
    Given a user has arrived on the privacy page
    Then the 'Print this page' link is not displayed

  Scenario: key stages page does not have a 'Print this page' link
    Given a user has arrived on the 'Which key stages' page
    Then the 'Print this page' link is not displayed

  Scenario: subjects page does not have a 'Print this page' link
    Given a user has arrived on the 'Which subjects' page for 'Key stage 2'
    Then the 'Print this page' link is not displayed

  Scenario: search results page has a 'Print this page' link
    Given a user has arrived on the 'Search results' page for 'Key stage 2 Maths'
    Then the 'Print this page' link is displayed

  Scenario: search results page 'Print this page' link opens print dialog
    Given a user has arrived on the 'Search results' page for 'Key stage 2 Maths'
    When the 'Print this page' link is clicked
    Then the print dialog is opened

  Scenario: tuition partner details page has a 'Print this page' link
    Given a user has arrived on the 'Tuition Partner' page for tp name 2
    Then the 'Print this page' link is displayed

  Scenario: tuition partner details page 'Print this page' link opens print dialog
    Given a user has arrived on the 'Tuition Partner' page for tp name 2

    When the 'Print this page' link is clicked
    Then the print dialog is opened

  Scenario: all quality-assured tuition partners page has a 'Print this page' link
    Given a user has arrived on the all quality-assured tuition partners page
    Then the 'Print this page' link is displayed

  Scenario: all quality-assured tuition partners page 'Print this page' link opens print dialog
    Given a user has arrived on the all quality-assured tuition partners page
    When the 'Print this page' link is clicked
    Then the print dialog is opened