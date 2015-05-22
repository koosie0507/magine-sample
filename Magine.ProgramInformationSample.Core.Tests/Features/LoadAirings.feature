Feature: LoadAirings
    As a logged in user I'd like to get some airings

Scenario: Logged in user requests airings
    Given I am logged in
    When I request airings
    Then I should receive a series of airings

Scenario: User who hasn't logged in requests airings
    Given I am a non-authenticated user
    When I request airings
    Then I am redirected to the login page
