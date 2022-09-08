Feature: User can print relevant pages using a 'Print this page' link
  Scenario: start page does not have a 'Print this page' link
    Given a user has started the 'Find a tuition partner' journey
    Then the 'Print this page' link is not displayed

  Scenario: funding and reporting page does not have a 'Print this page' link
    Given a user has arrived on the funding and reporting page
    Then the 'Print this page' link is not displayed

  Scenario: academic mentors page does not have a 'Print this page' link
    Given a user has arrived on the academic mentors page
    Then the 'Print this page' link is not displayed
    
  Scenario: school led tutoring page does not have a 'Print this page' link
    Given a user has arrived on the school led tutoring page
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

  Scenario: search results page does not have a 'Print this page' link
    Given a user has arrived on the 'Search results' page for 'Key stage 2 Maths'
    Then the 'Print this page' link is not displayed

  Scenario: tuition partner details page has a 'Print this page' link
    Given a user has arrived on the 'Tuition Partner' page for 'Career Tree'
    Then the 'Print this page' link is displayed

  Scenario: 'Print this page' link opens print dialog
    Given a user has arrived on the 'Tuition Partner' page for 'Career Tree'
    When the 'Print this page' link is clicked
    Then the print dialog is opened