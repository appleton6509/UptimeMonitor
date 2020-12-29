import React, { Component } from 'react';
import { Collapse, Container, Navbar,Button, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';

export class NavMenu extends Component {
    static displayName = NavMenu.name;

    constructor(props) {
        super(props);
        this.state = {
            collapsed: true
        };
    }

    toggleNavbar = () =>{
        this.setState({
            collapsed: !this.state.collapsed
        });
    }

    render() {
        return (
            <header>    
                <Navbar className="navbar-expand-sm navbar-toggleable-sm bg-white border-bottom box-shadow" light>
                    <Container>
                        <NavbarBrand tag={Link} to="/">Uptime Monitor</NavbarBrand>
                        <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
                        <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
                            <ul className="navbar-nav flex-grow">
                                <NavItem>
                                    <NavLink tag={Link} className="text-dark" to="/ManageEndPoints">EndPoints</NavLink>
                                </NavItem>
                                 <NavItem>
                                    <NavLink tag={Link} className="text-dark" to="/">Dashboard</NavLink>
                                </NavItem>
                                <NavItem>
                                    <NavLink tag={Link} className="text-dark" to="/SignIn">Login</NavLink>
                                </NavItem>
                                <NavItem>
                                <Button tag={Link} className="btn-success" to="/SignUp">Get Started</Button>
                                </NavItem>
                            </ul>
                        </Collapse>
                        
                    </Container>
                </Navbar>
            </header>
        );
    }
}
