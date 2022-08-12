Feature: Privacy Notice Page Tests
 Scenario: page as privacy notice header
    Given a user has arrived on the privacy page
    Then they will see the privacy notice header

 Scenario: page as link to personal information link
    Given a user has arrived on the privacy page
    Then they will see personal information link
    And the personal information link opens in a new window

 Scenario: page as link to contact form
    Given a user has arrived on the privacy page
    Then they will see contact form link
    And the contact form link opens in a new window

 Scenario: page as link to contact dfe form
    Given a user has arrived on the privacy page
    Then they will see contact form dfe link
    And the contact dfe form link opens in a new window

 Scenario: page as link to information commissioner 
    Given a user has arrived on the privacy page
    Then they will see information commissioner link
    And the contact information commissioner link opens in a new window

 Scenario: page as link to dfe contact form 
    Given a user has arrived on the privacy page
    Then they will see contact dfe contact link
    And the contact dfe link opens in a new window

 Scenario: page as link to contact secure dfe
    Given a user has arrived on the privacy page
    Then they will see contact secure dfe form link
    And the contact secure dfe link opens in a new window

 Scenario: page as link to contact secure dfe
    Given a user has arrived on the privacy page
    Then they will see contact secure dfe online form link
    And the contact secure dfe online form link opens in a new window

  Scenario: page as link to home link
     Given a user has arrived on the privacy page
     Then they will see the home link


