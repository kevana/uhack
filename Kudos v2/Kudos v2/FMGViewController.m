//
//  FMGViewController.m
//  Kudos v2
//
//  Created by Connor Dunne on 9/27/14.
//  Copyright (c) 2014 Fighting Mongooses. All rights reserved.
//

#import "FMGViewController.h"

@interface FMGViewController ()
@property (weak, nonatomic) IBOutlet UIWebView *webView;

@end

@implementation FMGViewController

- (void)viewDidLoad
{
    [super viewDidLoad];
    NSString *fullURL = @"http://www.umn.edu";
    NSURL *url = [[NSURL alloc] initWithString:fullURL];
    NSURLRequest *requestObj = [NSURLRequest requestWithURL:url];
    [_webView loadRequest:requestObj];
	// Do any additional setup after loading the view, typically from a nib.
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

@end
