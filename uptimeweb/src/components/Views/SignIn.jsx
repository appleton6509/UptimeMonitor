import React, { Component } from 'react';
import { Card, Button, Container, Row, Col, CardBody, Form, 
    FormGroup, Label, Input,PopoverBody, UncontrolledPopover } from 'reactstrap';
import authservice from '../Services/authservice'


export class SignIn extends Component {
    static displayName = SignIn.name;
    constructor(props) {
        super(props);
        this.state = {
            Username: "",
            Password: "",
            responseMessage: "",
            popoverOpen: false,
        }
    }

    setPopoverOpen = () => {
        this.setState({ popoverOpen: !this.popoverOpen })
    }
    
    onSubmit = async (event) => {
        event.preventDefault();
        let result = await new authservice().signIn(this.state.Username,this.state.Password);
        if (!result.message && result.tokenReceived)
            this.setState({responseMessage: "Success"});
        else 
            this.setState({responseMessage: result.message});

        //do something
    }

    handleUserChange = (e) => {
        this.setState({ Username: e.target.value })
    }
    handlePasswordChange = (e) => {
        this.setState({ Password: e.target.value })
    }

    render() {
        return (
            <Container>
                <Row>
                    <Col>
                        <h1 className="mt-3 mb-3 text-center">
                        Lets get you LOGGED IN.
                        </h1> 
                    </Col>
                </Row>
                <Row className="justify-content-center" >
                    <Col md="6" >
                        <Card className="shadow mt-4">
                            <CardBody>
                                <Form onSubmit={this.onSubmit}>
                                    <FormGroup>
                                        <Label>Email / UserName</Label>
                                        <Input type="text" id="username" name="username" formNoValidate required={false} placeholder="email address" onChange={this.handleUserChange} />
                                    </FormGroup>
                                    <FormGroup>
                                        <Label>Password</Label>
                                        <Input type="password" id="password" name="password" formNoValidate required={false} placeholder="strong password goes here" onChange={this.handlePasswordChange} />
                                    </FormGroup>
                                    <FormGroup className="text-center">
                                        <Button type="submit" id="btnSubmit">OK</Button>
                                    </FormGroup>
                                </Form>
                                <UncontrolledPopover isOpen={this.state.popoverOpen} placement="bottom"
                                    toggle={this.setPopoverOpen} target="btnSubmit">
                                    <PopoverBody>{this.state.responseMessage}</PopoverBody>
                                </UncontrolledPopover>
                            </CardBody>
                        </Card>
                    </Col>
                </Row>
            </Container>
        )
    }

}