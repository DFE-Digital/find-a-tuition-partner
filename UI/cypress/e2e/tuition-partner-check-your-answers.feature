Feature: Check your answers page for tuition partner response

    Scenario: Confirmation Page has the expected structure and data
        Given a user has started the 'Find a tuition partner' journey
        When a tuition partner clicks the magic link to respond to a schools enquiry
        Then Tuition partner has journeyed to the check your answers page
        And  the user has arrived on the tuition response check your answers page
        Then the heading of the page has text 'Check your answers before sending your response'


    Scenario: Back button redirects to the enquiry response page
        Given a user has started the 'Find a tuition partner' journey
        When a tuition partner clicks the magic link to respond to a schools enquiry
        Then Tuition partner has journeyed to the check your answers page
        And  the user has arrived on the tuition response check your answers page
        When they click 'Back'
        Then the user has arrived on the tuition response page

    Scenario: Verify Check Your Answers page content
        Given a user has started the 'Find a tuition partner' journey
        When a tuition partner clicks the magic link to respond to a schools enquiry
        Then Tuition partner has journeyed to the check your answers page
        And  the user has arrived on the tuition response check your answers page
        Then the Tuition Partner Check Your Answers page displays the following:
            | Section Name                                                                          | Expected Content |
            | Key stage and subjects: Key stage 1: English and Maths Key stage 2: English and Maths | 80               |
            | Type type:                                                                            | 80               |
            | Tuition plan:                                                                         | 80               |
            | SEND requirements:                                                                    | 80               |
            | Other school considerations:                                                          | 80               |

    Scenario: Change selections for Key Stages and Subjects on TP response
        Given a user has started the 'Find a tuition partner' journey
        When a tuition partner clicks the magic link to respond to a schools enquiry
        Then Tuition partner has journeyed to the check your answers page
        And  the user has arrived on the tuition response check your answers page
        When the user clicks the change button '1'
        Then the user has arrived on the tuition response page
        When they type '5' characters for section 1
        And they click 'Continue'
        Then the user has arrived on the tuition response check your answers page
        And the Tuition Partner Check Your Answers page displays the following:
            | Section Name                                                                          | Expected Content |
            | Key stage and subjects: Key stage 1: English and Maths Key stage 2: English and Maths | aaaaa            |
            | Type type:                                                                            | 80               |
            | Tuition plan:                                                                         | 80               |
            | SEND requirements:                                                                    | 80               |
            | Other school considerations:                                                          | 80               |

    Scenario: Change selections for Tuition type on TP response
        Given a user has started the 'Find a tuition partner' journey
        When a tuition partner clicks the magic link to respond to a schools enquiry
        Then Tuition partner has journeyed to the check your answers page
        And  the user has arrived on the tuition response check your answers page
        When the user clicks the change button '2'
        Then the user has arrived on the tuition response page
        When they type '2' characters for section 2
        And they click 'Continue'
        Then the user has arrived on the tuition response check your answers page
        And the Tuition Partner Check Your Answers page displays the following:
            | Section Name                                                                          | Expected Content |
            | Key stage and subjects: Key stage 1: English and Maths Key stage 2: English and Maths | 80               |
            | Type type:                                                                            | aa               |
            | Tuition plan:                                                                         | 80               |
            | SEND requirements:                                                                    | 80               |
            | Other school considerations:                                                          | 80               |


    Scenario: Change selections for Tuition plan on TP response
        Given a user has started the 'Find a tuition partner' journey
        When a tuition partner clicks the magic link to respond to a schools enquiry
        Then Tuition partner has journeyed to the check your answers page
        And  the user has arrived on the tuition response check your answers page
        When the user clicks the change button '3'
        Then the user has arrived on the tuition response page
        When they type '6' characters for section 3
        And they click 'Continue'
        Then the user has arrived on the tuition response check your answers page
        And the Tuition Partner Check Your Answers page displays the following:
            | Section Name                                                                          | Expected Content |
            | Key stage and subjects: Key stage 1: English and Maths Key stage 2: English and Maths | 80               |
            | Type type:                                                                            | 80               |
            | Tuition plan:                                                                         | aaaaaa           |
            | SEND requirements:                                                                    | 80               |
            | Other school considerations:                                                          | 80               |


    Scenario: Change selections for SEND Requirements on TP response
        Given a user has started the 'Find a tuition partner' journey
        When a tuition partner clicks the magic link to respond to a schools enquiry
        Then Tuition partner has journeyed to the check your answers page
        And  the user has arrived on the tuition response check your answers page
        When the user clicks the change button '4'
        Then the user has arrived on the tuition response page
        When they type '4' characters for section 4
        And they click 'Continue'
        Then the user has arrived on the tuition response check your answers page
        And the Tuition Partner Check Your Answers page displays the following:
            | Section Name                                                                          | Expected Content |
            | Key stage and subjects: Key stage 1: English and Maths Key stage 2: English and Maths | 80               |
            | Type type:                                                                            | 80               |
            | Tuition plan:                                                                         | 80               |
            | SEND requirements:                                                                    | aaaa             |
            | Other school considerations:                                                          | 80               |



    Scenario: Change selections for Other school considerations on TP response
        Given a user has started the 'Find a tuition partner' journey
        When a tuition partner clicks the magic link to respond to a schools enquiry
        Then Tuition partner has journeyed to the check your answers page
        And  the user has arrived on the tuition response check your answers page
        When the user clicks the change button '5'
        Then the user has arrived on the tuition response page
        When they type '1' characters for section 5
        And they click 'Continue'
        Then the user has arrived on the tuition response check your answers page
        And the Tuition Partner Check Your Answers page displays the following:
            | Section Name                                                                          | Expected Content |
            | Key stage and subjects: Key stage 1: English and Maths Key stage 2: English and Maths | 80               |
            | Type type:                                                                            | 80               |
            | Tuition plan:                                                                         | 80               |
            | SEND requirements:                                                                    | 80               |
            | Other school considerations:                                                          | a                |


    Scenario: Cancelling change selections for Key Stages and Subjects on TP response by clicking back
        Given a user has started the 'Find a tuition partner' journey
        When a tuition partner clicks the magic link to respond to a schools enquiry
        Then Tuition partner has journeyed to the check your answers page
        And  the user has arrived on the tuition response check your answers page
        When the user clicks the change button '1'
        Then the user has arrived on the tuition response page
        When they click back on the browser
        Then the user has arrived on the tuition response check your answers page
        And the Tuition Partner Check Your Answers page displays the following:
            | Section Name                                                                          | Expected Content |
            | Key stage and subjects: Key stage 1: English and Maths Key stage 2: English and Maths | 80               |
            | Type type:                                                                            | 80               |
            | Tuition plan:                                                                         | 80               |
            | SEND requirements:                                                                    | 80               |
            | Other school considerations:                                                          | 80               |

    Scenario: Clicking the contact us link and going back redirects back to the check your answers page
        Given a user has started the 'Find a tuition partner' journey
        When a tuition partner clicks the magic link to respond to a schools enquiry
        Then Tuition partner has journeyed to the check your answers page
        And  the user has arrived on the tuition response check your answers page
        When they click the contact us link
        And a user has arrived on the contact us page
        When they click back on the browser
        Then the user has arrived on the tuition response check your answers page

    Scenario: Submitting a successful response shows the confirmation page correctly
        Given a user has started the 'Find a tuition partner' journey
        When a tuition partner clicks the magic link to respond to a schools enquiry
        Then Tuition partner has journeyed to the check your answers page
        When  the user has arrived on the tuition response check your answers page
        And they click 'Submit'
        Then the user has arrived on the tuition response confirmation page
        And the page has title 'Response sent'
        And a unique reference number is shown