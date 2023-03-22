Feature: View all responses page


    Scenario: Submitting a successful response shows the confirmation page correctly
        Given a user has started the 'Find a tuition partner' journey
        When a tuition partner clicks the magic link to respond to a schools enquiry
        Then Tuition partner has journeyed to the check your answers page
        When  the user has arrived on the tuition response check your answers page
        And they click 'Submit'
        Then the user has arrived on the tuition response confirmation page
        And the page has title 'Response sent'
        And a unique reference number is shown


    Scenario: View all responses page to have the expected structure
        Given A school has arrived on view all responses page with a response
        When the user has arrived on the view all enquiry responses page
        Then the heading of the page is 'View responses to your tuition enquiry'
        And there is text 'Your tuition requirements'
        When the user clicks Your tuition requirements
        Then The correct enquiry information is shown as follows:
            | Enquiry information                             |
            | This is the tuition requirements you asked for: |
            | Key stage 1: English and Maths                  |
            | Key stage 2: English and Maths                  |
            | Any                                             |
            | enquiry                                         |
            | enquiry                                         |
            | enquiry                                         |
        And there should be text stating the amount of tuition partners that responded


    Scenario: A tuition partners name and response date is shown on view all responses page
        Given A school has arrived on view all responses page with a response
        Then The tuition partner responses are shown at the bottom of the page as follows:
            | Column          | Value                            |
            | Date            |                                  |
            | Tuition Partner | Sherpa Online                    |
            | Their Response  | View response from Sherpa Online |

    Scenario: Clicking on a Tuition Partners Response redirects to their response page
        Given A school has arrived on view all responses page with a response
        And they click View response from a tuition partner
        Then the tuition partners response page is shown

    Scenario: Tuiton Partners Response Page has the expected structure
        Given A school has arrived on view all responses page with a response
        And they click View response from a tuition partner
        When they click all the view your tuition requirement links the text shows
        Then the response page has the following information:
            | Requirement                    | Your Response                  | Sherpa Online's Response |
            | Key stage and subjects:        | Key stage 1: English and Maths | 80                       |
            | Key stage and subjects:        | Key stage 2: English and Maths | 80                       |
            | Tuition type:                  | Any                            | 80                       |
            | Tuition plan:                  | enquiry                        | 80                       |
            | Can you support SEND?:         | enquiry                        | 80                       |
            | Other tuition considerations?: | enquiry                        | 80                       |


    Scenario: Contact us page has the expected content structure
        Given A school has arrived on view all responses page with a response
        And they click View response from a tuition partner
        And they click contact tuition partner
        Then the user has arrived on the contact tuition partner page
        And the heading of the page is 'Contact Sherpa Online'
        Then the page has the correct content information
        And the page shows contact information such as the following:
            | Section Name    | Value                    |
            | email:          | angela@sherpa-online.com |
            | contact number: | 01628 337 590            |

    Scenario: Clicking back redirects to tuition partner's response page
        Given A school has arrived on view all responses page with a response
        And they click View response from a tuition partner
        And they click contact tuition partner
        Then the user has arrived on the contact tuition partner page
        When they click 'Back'
        Then the tuition partners response page is shown

    Scenario: Clicking return to enquiry list returns to all enquiries page
        Given A school has arrived on view all responses page with a response
        And they click View response from a tuition partner
        And they click contact tuition partner
        Then the user has arrived on the contact tuition partner page
        When they click return to your enquiry list
        Then the user has arrived on the view all enquiry responses page