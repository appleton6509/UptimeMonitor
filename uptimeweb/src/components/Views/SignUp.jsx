import React, { Component } from 'react';
import { Card, Button, Container, Row, Col, CardBody, Form, 
    FormGroup, Label, Input,PopoverBody, UncontrolledPopover } from 'reactstrap';


export class SignUp extends Component {
    static displayName = SignUp.name;
    constructor(props) {
        super(props);
        this.state = {
            Username: "",
            Password: "",
            httperror: "",
            popoverOpen: false
        }
    }

    setPopoverOpen = () => {
        this.setState({ popoverOpen: !this.popoverOpen })
    }
    postUser = async (jsonbody, uri) => {
        this.setState({ error: "" });
        await fetch(uri, {
            method: 'POST',
            body: jsonbody,
            headers: {
                'Accept': '*/*',
                'Content-Type': 'application/json'
            }
        }).then(res => {
            if (!res.ok)
                return res.text()
        }).then(message => {
            this.setState({ httperror: message })
        }).catch(err => {
            this.setState({ httperror: "something went wrong" })
        });
    }
    onSubmit = async (event) => {
        event.preventDefault();
        const body = {
            Username: this.state.Username,
            Password: this.state.Password
        }
        const uri = 'https://localhost:44373/api/Auth/SignUp';
        await this.postUser(JSON.stringify(body), uri);
        if (this.state.httperror) {
            console.log(this.state.httperror);
        }
        //else 
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
                        Create a FREE Account.
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
                                        <Input type="text" formNoValidate required={false} id="username" name="username" placeholder="email address" onChange={this.handleUserChange} />
                                    </FormGroup>
                                    <FormGroup>
                                        <Label>Password</Label>
                                        <Input type="password" formNoValidate required={false} id="password" name="password"  placeholder="strong password goes here" onChange={this.handlePasswordChange} />
                                    </FormGroup>
                                    <FormGroup className="text-center">
                                        <Button type="submit" id="btnSubmit" className="mb-4" >OK</Button>
                                    </FormGroup>
                                </Form>
                                <UncontrolledPopover isOpen={this.state.popoverOpen} trigger="focus click" placement="bottom"
                                    toggle={this.setPopoverOpen} target="btnSubmit">
                                    <PopoverBody>{this.state.httperror}</PopoverBody>
                                </UncontrolledPopover>
                            </CardBody>
                        </Card>
                    </Col>
                </Row>

            </Container>
        )
    }

}